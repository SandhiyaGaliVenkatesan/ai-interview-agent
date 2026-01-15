using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;

namespace InterviewAgent.Api.Controllers;

[ApiController]
[Route("api/gemini")]
public class GeminiDebugController : ControllerBase
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public GeminiDebugController(IHttpClientFactory factory, IConfiguration config)
    {
        _http = factory.CreateClient();
        _config = config;
    }

    [HttpGet("models")]
    public async Task<IActionResult> ListModels()
    {
        var apiKeyEnv = _config["Ai:Gemini:ApiKeyEnv"] ?? "GEMINI_API_KEY";
        var apiKey = Environment.GetEnvironmentVariable(apiKeyEnv);

        if (string.IsNullOrWhiteSpace(apiKey))
            return BadRequest($"Missing API key env var: {apiKeyEnv}");

        var url = $"https://generativelanguage.googleapis.com/v1beta/models?key={apiKey}";
        var res = await _http.GetAsync(url);
        var text = await res.Content.ReadAsStringAsync();
        return Content(text, "application/json");
    }
}
