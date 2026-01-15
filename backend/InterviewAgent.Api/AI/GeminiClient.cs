using System.Net.Http.Json;
using System.Text.Json;

namespace InterviewAgent.Api.Services.Ai;

public class GeminiClient : IAiClient
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly PromptLoader _prompts;

    public GeminiClient(HttpClient http, IConfiguration config, PromptLoader prompts)
    {
        _http = http;
        _config = config;
        _prompts = prompts;
    }

    public async Task<string> GenerateQuestionJsonAsync(string track, string difficulty, string? context)
    {
        var template = _prompts.Load("question.prompt.txt");
        var prompt = template
            .Replace("{TRACK}", track)
            .Replace("{DIFFICULTY}", difficulty)
            .Replace("{CONTEXT}", context ?? "");

        return default;
    }

    public async Task<string> EvaluateAnswerJsonAsync(string track, string difficulty, string questionJson, string userAnswer)
    {
        var template = _prompts.Load("evaluate.prompt.txt");
        var prompt = template
            .Replace("{TRACK}", track)
            .Replace("{DIFFICULTY}", difficulty)
            .Replace("{QUESTION_JSON}", questionJson)
            .Replace("{ANSWER}", userAnswer ?? "");

        //return await GenerateViaFunctionCallAsync(prompt, mode: SchemaMode.Evaluation);
        return default;
    }
}