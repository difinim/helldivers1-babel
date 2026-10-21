using System.Net;
using Babel.Core.Models;
using Babel.Providers.Gemini;
using Babel.Providers.Tests.Mocks;

namespace Babel.Providers.Tests;

public class GeminiProviderTests
{
    private static BabelConfig CreateDefaultConfig(string apiKey = "TEST_API_KEY", string model = "gemini-flash-lite-latest")
    {
        var config = new BabelConfig
        {
            TimeoutSeconds = 5.0
        };
        config.ApiKeys["Gemini"] = apiKey;
        config.ProviderModels["Gemini"] = model;
        return config;
    }

    [Fact]
    public async Task TranslateAsync_IncomingMessage_ParsesSuccessfully_ExtractsDetectedLanguage()
    {
        // Arrange
        const string responseJson = """
        {
          "candidates": [
            {
              "content": {
                "parts": [
                  { "text": "(English): Watch out for the bug patrol!" }
                ],
                "role": "model"
              },
              "finishReason": "STOP",
              "index": 0
            }
          ]
        }
        """;

        var mockHandler = new MockHttpMessageHandler(HttpStatusCode.OK, responseJson);
        using var httpClient = new HttpClient(mockHandler);
        using var provider = new GeminiProvider(httpClient);

        var config = CreateDefaultConfig();
        var request = new TranslationRequest
        {
            Text = "Watch out for the bug patrol!",
            TargetLanguage = "Portuguese (PT-BR)",
            IsOutgoing = false
        };

        // Act
        var result = await provider.TranslateAsync(request, config);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("(English): Watch out for the bug patrol!", result.TranslatedText);
        Assert.Equal("English", result.DetectedSourceLanguage);
        Assert.Null(result.ErrorMessage);
        Assert.True(result.LatencyMs >= 0);

        // Verify request payload
        Assert.NotNull(mockHandler.LastRequest);
        Assert.Contains("models/gemini-flash-lite-latest:generateContent?key=TEST_API_KEY", mockHandler.LastRequest.RequestUri?.ToString());
        Assert.NotNull(mockHandler.LastRequestBody);
        Assert.Contains("Translate in-game chat for the Helldivers game to Portuguese (PT-BR)", mockHandler.LastRequestBody);
        Assert.DoesNotContain("thinkingConfig", mockHandler.LastRequestBody);
    }

    [Fact]
    public async Task TranslateAsync_OutgoingMessage_ParsesSuccessfully()
    {
        // Arrange
        const string responseJson = """
        {
          "candidates": [
            {
              "content": {
                "parts": [
                  { "text": "отступаем сейчас!" }
                ],
                "role": "model"
              },
              "finishReason": "STOP"
            }
          ]
        }
        """;

        var mockHandler = new MockHttpMessageHandler(HttpStatusCode.OK, responseJson);
        using var httpClient = new HttpClient(mockHandler);
        using var provider = new GeminiProvider(httpClient);

        var config = CreateDefaultConfig();
        var request = new TranslationRequest
        {
            Text = "vamos recuar agora",
            TargetLanguage = "Russian",
            IsOutgoing = true
        };

        // Act
        var result = await provider.TranslateAsync(request, config);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("отступаем сейчас!", result.TranslatedText);
        Assert.Null(result.ErrorMessage);

        // Verify prompt
        Assert.NotNull(mockHandler.LastRequestBody);
        Assert.Contains("Target language: Russian. Message: vamos recuar agora", mockHandler.LastRequestBody);
        Assert.Contains("Casual, lowercase, keep acronyms uppercase", mockHandler.LastRequestBody);
    }

    [Fact]
    public async Task TranslateAsync_StripEnclosingQuotes_RemovesQuotes()
    {
        // Arrange
        const string responseJson = """
        {
          "candidates": [
            {
              "content": {
                "parts": [
                  { "text": "\"cuidado com o tanque\"" }
                ]
              }
            }
          ]
        }
        """;

        var mockHandler = new MockHttpMessageHandler(HttpStatusCode.OK, responseJson);
        using var httpClient = new HttpClient(mockHandler);
        using var provider = new GeminiProvider(httpClient);

        var config = CreateDefaultConfig();
        var request = new TranslationRequest
        {
            Text = "watch out for tank",
            TargetLanguage = "Portuguese (PT-BR)",
            IsOutgoing = true
        };

        // Act
        var result = await provider.TranslateAsync(request, config);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("cuidado com o tanque", result.TranslatedText);
    }

    [Fact]
    public async Task TranslateAsync_CustomModel_UsesConfiguredModelInUrl()
    {
        // Arrange
        const string responseJson = """
        {
          "candidates": [
            {
              "content": {
                "parts": [
                  { "text": "test" }
                ]
              }
            }
          ]
        }
        """;

        var mockHandler = new MockHttpMessageHandler(HttpStatusCode.OK, responseJson);
        using var httpClient = new HttpClient(mockHandler);
        using var provider = new GeminiProvider(httpClient);

        var config = CreateDefaultConfig(model: "gemini-2.0-flash-lite");
        var request = new TranslationRequest
        {
            Text = "test",
            TargetLanguage = "English",
            IsOutgoing = false
        };

        // Act
        await provider.TranslateAsync(request, config);

        // Assert
        Assert.NotNull(mockHandler.LastRequest);
        Assert.Contains("models/gemini-2.0-flash-lite:generateContent", mockHandler.LastRequest.RequestUri?.ToString());
    }

    [Fact]
    public async Task TranslateAsync_MissingApiKey_ReturnsConfigurationError()
    {
        // Arrange
        var mockHandler = new MockHttpMessageHandler(HttpStatusCode.OK, "{}");
        using var httpClient = new HttpClient(mockHandler);
        using var provider = new GeminiProvider(httpClient);

        var config = CreateDefaultConfig(apiKey: "");
        var request = new TranslationRequest
        {
            Text = "Hello",
            TargetLanguage = "English"
        };

        // Act
        var result = await provider.TranslateAsync(request, config);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("API key is not configured", result.ErrorMessage);
        Assert.Equal(0, mockHandler.CallCount);
    }

    [Fact]
    public async Task TranslateAsync_EmptyPrompt_ReturnsFailureWithoutCallingHttp()
    {
        // Arrange
        var mockHandler = new MockHttpMessageHandler(HttpStatusCode.OK, "{}");
        using var httpClient = new HttpClient(mockHandler);
        using var provider = new GeminiProvider(httpClient);

        var config = CreateDefaultConfig();
        var request = new TranslationRequest
        {
            Text = "   ",
            TargetLanguage = "English"
        };

        // Act
        var result = await provider.TranslateAsync(request, config);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Empty message prompt", result.ErrorMessage);
        Assert.Equal(0, mockHandler.CallCount);
    }

    [Fact]
    public async Task TranslateAsync_Http404_ReturnsNormalizedModelError()
    {
        // Arrange
        const string errorJson = """
        {
          "error": {
            "code": 404,
            "message": "models/gemini-2.5-flash-lite is not found for API version v1beta",
            "status": "NOT_FOUND"
          }
        }
        """;

        var mockHandler = new MockHttpMessageHandler(HttpStatusCode.NotFound, errorJson);
        using var httpClient = new HttpClient(mockHandler);
        using var provider = new GeminiProvider(httpClient);

        var config = CreateDefaultConfig();
        var request = new TranslationRequest
        {
            Text = "Hello world",
            TargetLanguage = "Portuguese (PT-BR)"
        };

        // Act
        var result = await provider.TranslateAsync(request, config);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("404", result.ErrorMessage);
        Assert.Contains("gemini-2.5-flash-lite is not found", result.ErrorMessage);
    }

    [Fact]
    public async Task TranslateAsync_Http429_ReturnsNormalizedRateLimitError()
    {
        // Arrange
        const string errorJson = """
        {
          "error": {
            "code": 429,
            "message": "Resource has been exhausted (e.g. check quota)",
            "status": "RESOURCE_EXHAUSTED"
          }
        }
        """;

        var mockHandler = new MockHttpMessageHandler(HttpStatusCode.TooManyRequests, errorJson);
        using var httpClient = new HttpClient(mockHandler);
        using var provider = new GeminiProvider(httpClient);

        var config = CreateDefaultConfig();
        var request = new TranslationRequest
        {
            Text = "Watch out",
            TargetLanguage = "Portuguese (PT-BR)"
        };

        // Act
        var result = await provider.TranslateAsync(request, config);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Rate limit exceeded", result.ErrorMessage);
        Assert.Contains("429", result.ErrorMessage);
    }

    [Fact]
    public async Task TranslateAsync_Http503_ReturnsNormalizedCapacityError()
    {
        // Arrange
        const string errorJson = """
        {
          "error": {
            "code": 503,
            "message": "The model is overloaded. Please try again later.",
            "status": "UNAVAILABLE"
          }
        }
        """;

        var mockHandler = new MockHttpMessageHandler(HttpStatusCode.ServiceUnavailable, errorJson);
        using var httpClient = new HttpClient(mockHandler);
        using var provider = new GeminiProvider(httpClient);

        var config = CreateDefaultConfig();
        var request = new TranslationRequest
        {
            Text = "Watch out",
            TargetLanguage = "Portuguese (PT-BR)"
        };

        // Act
        var result = await provider.TranslateAsync(request, config);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("503", result.ErrorMessage);
        Assert.Contains("temporarily unavailable", result.ErrorMessage);
    }

    [Fact]
    public async Task TranslateAsync_Http401_ReturnsAuthenticationError()
    {
        // Arrange
        const string errorJson = """
        {
          "error": {
            "code": 401,
            "message": "API key not valid. Please pass a valid API key.",
            "status": "UNAUTHENTICATED"
          }
        }
        """;

        var mockHandler = new MockHttpMessageHandler(HttpStatusCode.Unauthorized, errorJson);
        using var httpClient = new HttpClient(mockHandler);
        using var provider = new GeminiProvider(httpClient);

        var config = CreateDefaultConfig();
        var request = new TranslationRequest
        {
            Text = "Watch out",
            TargetLanguage = "Portuguese (PT-BR)"
        };

        // Act
        var result = await provider.TranslateAsync(request, config);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Authentication failed", result.ErrorMessage);
        Assert.Contains("401", result.ErrorMessage);
    }

    [Fact]
    public async Task TranslateAsync_Timeout_ReturnsTimedOutError()
    {
        // Arrange
        var mockHandler = new MockHttpMessageHandler(async (_, ct) =>
        {
            await Task.Delay(2000, ct);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        using var httpClient = new HttpClient(mockHandler);
        using var provider = new GeminiProvider(httpClient);

        var config = CreateDefaultConfig();
        config.TimeoutSeconds = 0.05; // 50ms timeout

        var request = new TranslationRequest
        {
            Text = "Watch out",
            TargetLanguage = "Portuguese (PT-BR)"
        };

        // Act
        var result = await provider.TranslateAsync(request, config);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("timed out", result.ErrorMessage);
    }

    [Fact]
    public async Task TranslateAsync_CallerCancellation_ReturnsCancelledError()
    {
        // Arrange
        var mockHandler = new MockHttpMessageHandler(HttpStatusCode.OK, "{}");
        using var httpClient = new HttpClient(mockHandler);
        using var provider = new GeminiProvider(httpClient);

        var config = CreateDefaultConfig();
        var request = new TranslationRequest
        {
            Text = "Watch out",
            TargetLanguage = "Portuguese (PT-BR)"
        };

        using var cts = new CancellationTokenSource();
        cts.Cancel(); // Pre-cancel

        // Act
        var result = await provider.TranslateAsync(request, config, cts.Token);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("cancelled", result.ErrorMessage);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_ValidKey_ReturnsTrue()
    {
        // Arrange
        const string responseJson = """
        {
          "candidates": [
            {
              "content": {
                "parts": [
                  { "text": "hello" }
                ]
              }
            }
          ]
        }
        """;

        var mockHandler = new MockHttpMessageHandler(HttpStatusCode.OK, responseJson);
        using var httpClient = new HttpClient(mockHandler);
        using var provider = new GeminiProvider(httpClient);

        var config = CreateDefaultConfig();

        // Act
        var isValid = await provider.ValidateCredentialsAsync(config);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_InvalidKey_ReturnsFalse()
    {
        // Arrange
        const string errorJson = """
        {
          "error": {
            "code": 400,
            "message": "API key not valid."
          }
        }
        """;

        var mockHandler = new MockHttpMessageHandler(HttpStatusCode.BadRequest, errorJson);
        using var httpClient = new HttpClient(mockHandler);
        using var provider = new GeminiProvider(httpClient);

        var config = CreateDefaultConfig();

        // Act
        var isValid = await provider.ValidateCredentialsAsync(config);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_MissingKey_ReturnsFalseWithoutCallingHttp()
    {
        // Arrange
        var mockHandler = new MockHttpMessageHandler(HttpStatusCode.OK, "{}");
        using var httpClient = new HttpClient(mockHandler);
        using var provider = new GeminiProvider(httpClient);

        var config = CreateDefaultConfig(apiKey: "");

        // Act
        var isValid = await provider.ValidateCredentialsAsync(config);

        // Assert
        Assert.False(isValid);
        Assert.Equal(0, mockHandler.CallCount);
    }
}
