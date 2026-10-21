using System.Net;
using System.Text;

namespace Babel.Providers.Tests.Mocks;

/// <summary>
/// Mock HTTP message handler for intercepting requests and supplying simulated responses in tests.
/// </summary>
public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handler;

    public HttpRequestMessage? LastRequest { get; private set; }
    public string? LastRequestBody { get; private set; }
    public int CallCount { get; private set; }

    public MockHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler)
    {
        _handler = handler;
    }

    public MockHttpMessageHandler(HttpStatusCode statusCode, string content)
    {
        _handler = (_, _) =>
        {
            var response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };
            return Task.FromResult(response);
        };
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        CallCount++;
        LastRequest = request;
        if (request.Content != null)
        {
            LastRequestBody = await request.Content.ReadAsStringAsync(cancellationToken);
        }
        return await _handler(request, cancellationToken);
    }
}
