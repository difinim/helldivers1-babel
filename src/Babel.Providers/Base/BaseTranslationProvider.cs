using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Babel.Core.Interfaces;
using Babel.Core.Models;

namespace Babel.Providers.Base;

/// <summary>
/// Abstract base class for AI translation providers with HTTP resilience, stopwatch latency logging,
/// timeout cancellation, and error normalization.
/// </summary>
public abstract class BaseTranslationProvider : ITranslationProvider, IDisposable
{
    protected readonly HttpClient HttpClient;
    private readonly bool _disposeClient;

    public abstract string ProviderId { get; }
    public abstract string DisplayName { get; }

    protected BaseTranslationProvider(HttpClient? httpClient = null)
    {
        if (httpClient != null)
        {
            HttpClient = httpClient;
            _disposeClient = false;
        }
        else
        {
            HttpClient = new HttpClient();
            _disposeClient = true;
        }
    }

    /// <inheritdoc />
    public virtual async Task<TranslationResult> TranslateAsync(
        TranslationRequest request,
        BabelConfig config,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(config);

        if (ct.IsCancellationRequested)
        {
            return TranslationResult.Failed("Translation request was cancelled.");
        }

        if (string.IsNullOrWhiteSpace(request.Text))
        {
            return TranslationResult.Failed("Empty message prompt.");
        }

        var timeoutSeconds = config.TimeoutSeconds > 0 ? config.TimeoutSeconds : 5.0;
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = await TranslateCoreAsync(request, config, cts.Token).ConfigureAwait(false);
            stopwatch.Stop();
            result.LatencyMs = stopwatch.ElapsedMilliseconds;
            return result;
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            stopwatch.Stop();
            return TranslationResult.Failed("Translation request was cancelled.", stopwatch.ElapsedMilliseconds);
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            return TranslationResult.Failed($"Translation request timed out after {timeoutSeconds:F1}s.", stopwatch.ElapsedMilliseconds);
        }
        catch (HttpRequestException ex)
        {
            stopwatch.Stop();
            var errorMsg = NormalizeHttpException(ex);
            return TranslationResult.Failed(errorMsg, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return TranslationResult.Failed($"Translation error: {ex.Message}", stopwatch.ElapsedMilliseconds);
        }
    }

    /// <inheritdoc />
    public virtual async Task<bool> ValidateCredentialsAsync(BabelConfig config, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(config);

        if (!config.ApiKeys.TryGetValue(ProviderId, out var key) || string.IsNullOrWhiteSpace(key))
        {
            return false;
        }

        try
        {
            var testRequest = new TranslationRequest
            {
                Text = "Ping",
                TargetLanguage = "English",
                IsOutgoing = true
            };

            var result = await TranslateAsync(testRequest, config, ct).ConfigureAwait(false);
            return result.Success;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Executes provider-specific translation logic.
    /// </summary>
    protected abstract Task<TranslationResult> TranslateCoreAsync(
        TranslationRequest request,
        BabelConfig config,
        CancellationToken ct);

    /// <summary>
    /// Normalizes HTTP status codes and API error response bodies into friendly user messages.
    /// </summary>
    protected virtual string NormalizeHttpError(HttpStatusCode statusCode, string? responseBody = null)
    {
        var statusInt = (int)statusCode;
        var detail = !string.IsNullOrWhiteSpace(responseBody) ? ExtractErrorMessage(responseBody) : string.Empty;

        return statusCode switch
        {
            HttpStatusCode.TooManyRequests => // 429
                !string.IsNullOrEmpty(detail)
                    ? $"Rate limit exceeded (HTTP 429): {detail}. Check your API quota or wait a moment."
                    : "Rate limit exceeded (HTTP 429). Check your API quota or wait a moment before sending more messages.",

            HttpStatusCode.ServiceUnavailable => // 503
                !string.IsNullOrEmpty(detail)
                    ? $"AI service temporarily unavailable (HTTP 503): {detail}. The model is experiencing high demand."
                    : "AI service temporarily unavailable (HTTP 503). The model is experiencing high demand. Please try again shortly.",

            HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden => // 401, 403
                !string.IsNullOrEmpty(detail)
                    ? $"Authentication failed (HTTP {statusInt}): {detail}. Please verify your API key."
                    : $"Authentication failed (HTTP {statusInt}). Invalid or expired API key.",

            HttpStatusCode.NotFound => // 404
                !string.IsNullOrEmpty(detail)
                    ? $"Model not found or deprecated (HTTP 404): {detail}. Please verify your configured model."
                    : "Model not found or deprecated (HTTP 404). Please verify your configured model name.",

            HttpStatusCode.BadRequest => // 400
                !string.IsNullOrEmpty(detail)
                    ? $"Bad request (HTTP 400): {detail}."
                    : "Bad request (HTTP 400). Please check your request parameters.",

            HttpStatusCode.GatewayTimeout => // 504
                "Gateway timeout (HTTP 504). The upstream AI provider did not respond in time.",

            _ => !string.IsNullOrEmpty(detail)
                ? $"HTTP error ({statusInt}): {detail}"
                : $"HTTP error ({statusInt}): {statusCode}"
        };
    }

    /// <summary>
    /// Normalizes unhandled <see cref="HttpRequestException"/> instances.
    /// </summary>
    protected virtual string NormalizeHttpException(HttpRequestException ex)
    {
        if (ex.StatusCode.HasValue)
        {
            return NormalizeHttpError(ex.StatusCode.Value, ex.Message);
        }

        if (ex.InnerException is System.Net.Sockets.SocketException)
        {
            return $"Network connection failed: Unable to reach translation service ({ex.Message}).";
        }

        return $"Network error: {ex.Message}";
    }

    /// <summary>
    /// Attempts to extract user-facing error messages from API JSON response bodies.
    /// </summary>
    protected static string ExtractErrorMessage(string responseBody)
    {
        try
        {
            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;

            // Pattern: {"error": {"message": "..."}}
            if (root.TryGetProperty("error", out var errorElement))
            {
                if (errorElement.ValueKind == JsonValueKind.Object && errorElement.TryGetProperty("message", out var msgElement))
                {
                    var msg = msgElement.GetString();
                    if (!string.IsNullOrWhiteSpace(msg)) return msg;
                }
                else if (errorElement.ValueKind == JsonValueKind.String)
                {
                    var msg = errorElement.GetString();
                    if (!string.IsNullOrWhiteSpace(msg)) return msg;
                }
            }

            // Pattern: {"message": "..."}
            if (root.TryGetProperty("message", out var directMsg))
            {
                var msg = directMsg.GetString();
                if (!string.IsNullOrWhiteSpace(msg)) return msg;
            }
        }
        catch
        {
            // Response was not JSON
        }

        return string.Empty;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && _disposeClient)
        {
            HttpClient.Dispose();
        }
    }
}
