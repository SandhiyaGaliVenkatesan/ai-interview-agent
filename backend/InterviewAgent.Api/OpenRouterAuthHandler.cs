using System.Net.Http.Headers;

namespace InterviewAgent.Api.Services.Ai;

public sealed class OpenRouterAuthHandler : DelegatingHandler
{
    private readonly IConfiguration _config;

    public OpenRouterAuthHandler(IConfiguration config)
    {
        _config = config;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var key = (_config["Ai:OpenRouter:ApiKey"] ?? "").Trim().Trim('"');
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("OpenRouter:ApiKey is missing/empty.");

        // Force exact headers (like curl)
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", key);
        request.Headers.TryAddWithoutValidation("HTTP-Referer", _config["OpenRouter:Referer"] ?? "http://localhost:5173");
        request.Headers.TryAddWithoutValidation("X-Title", _config["OpenRouter:Title"] ?? "AI Interview Agent");

        return base.SendAsync(request, cancellationToken);
    }
}
