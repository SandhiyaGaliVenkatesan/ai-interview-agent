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
         var apiKeyRaw = (_config["Ai:OpenRouter:ApiKey"] ?? "");

         var apiKey = new string(apiKeyRaw.Where(c => c <= 127).ToArray())
            .Trim()
            .Trim('"');
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("Ai:OpenRouter:ApiKey is missing/empty at runtime.");

        // Force exact headers (like curl)
        request.Headers.Authorization =  new AuthenticationHeaderValue("Bearer", apiKey);
        request.Headers.TryAddWithoutValidation("HTTP-Referer", "http://localhost");
        request.Headers.TryAddWithoutValidation("X-Title", "ai-interview-agent");

        return base.SendAsync(request, cancellationToken);
    }
}
