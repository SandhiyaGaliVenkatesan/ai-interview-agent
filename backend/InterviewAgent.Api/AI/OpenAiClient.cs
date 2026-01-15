namespace InterviewAgent.Api.Services.Ai;

public class OpenAiClient : IAiClient
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly PromptLoader _prompts;

    public OpenAiClient(HttpClient http, IConfiguration config, PromptLoader prompts)
    {
        _http = http;
        _config = config;
        _prompts = prompts;
    }

    public Task<string> GenerateQuestionJsonAsync(string track, string difficulty, string? context)
        => throw new NotImplementedException("Wire OpenAI call here (kept as stub for scaffold).");

    public Task<string> EvaluateAnswerJsonAsync(string track, string difficulty, string questionJson, string userAnswer)
        => throw new NotImplementedException("Wire OpenAI call here (kept as stub for scaffold).");
}
