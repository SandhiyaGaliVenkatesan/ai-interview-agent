using System.Text;
using System.Text.Json;

namespace InterviewAgent.Api.Services.Ai;

public sealed class OpenRouterAiClient : IAiClient
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly PromptLoader _promptLoader;

    private static readonly string[] FallbackModels =
{
    "openai/gpt-oss-20b:free",
    "meta-llama/llama-3.1-8b-instruct:free",
    "mistralai/mistral-7b-instruct:free"
};


    public OpenRouterAiClient(HttpClient http, IConfiguration config, PromptLoader promptLoader)
{
    _http = http;
    _config = config;
    _promptLoader = promptLoader;
}

    // ---------------- Used by InterviewService ----------------

    public Task<string> GenerateQuestionJsonAsync(
        string track,
        string difficulty,
        string? context)
    {
        var template = _promptLoader.Load("question.prompt.txt");

       var prompt = template
        .Replace("{TRACK}", track, StringComparison.OrdinalIgnoreCase)
        .Replace("{DIFFICULTY}", difficulty, StringComparison.OrdinalIgnoreCase)
        .Replace("{CONTEXT}", context ?? "", StringComparison.OrdinalIgnoreCase);

        return SendAsync(prompt);
    }

    public Task<string> EvaluateAnswerJsonAsync(
        string track,
        string difficulty,
        string questionJson,
        string userAnswer)
    {
        var template = _promptLoader.Load("evaluate.prompt.txt");

        // var prompt = template
        //     .Replace("{{TRACK}}", track, StringComparison.OrdinalIgnoreCase)
        //     .Replace("{{DIFFICULTY}}", difficulty, StringComparison.OrdinalIgnoreCase)
        //     .Replace("{{QUESTION_JSON}}", questionJson, StringComparison.OrdinalIgnoreCase)
        //     .Replace("{{ANSWER}}", userAnswer, StringComparison.OrdinalIgnoreCase);

         var prompt = ApplyVars(
            template,
            track: track,
            difficulty: difficulty,
            context: "",
            questionJson: questionJson,
            answer: userAnswer
        );


        return SendAsync(prompt);
    }

    // ---------------- OpenRouter Call ----------------
private async Task<string> SendAsync(string prompt)
{
    var preferred = _config["Ai:OpenRouter:Model"];
    var models = string.IsNullOrWhiteSpace(preferred)
        ? FallbackModels
        : new[] { preferred }.Concat(FallbackModels).Distinct().ToArray();

    Exception? last = null;

    foreach (var model in models)
    {
        try
        {
            return await CallOpenRouterOnce(prompt, model);
        }
        catch (Exception ex) when (ex.Message.Contains("\"code\":429") || ex.Message.Contains("rate-limited"))
        {
            last = ex;
            continue; // try next model
        }
    }

    throw last ?? new Exception("All models failed.");
}

private static string ApplyVars(
        string template,
        string track,
        string difficulty,
        string context,
        string? questionJson = null,
        string? answer = null)
    {
        string R(string s, string from, string to)
            => s.Replace(from, to ?? "", StringComparison.OrdinalIgnoreCase);

        var t = template;

        // Curly style
        t = R(t, "{TRACK}", track);
        t = R(t, "{DIFFICULTY}", difficulty);
        t = R(t, "{CONTEXT}", context);

        if (questionJson != null) t = R(t, "{QUESTION_JSON}", questionJson);
        if (answer != null) t = R(t, "{ANSWER}", answer);

        // Double-curly style
        t = R(t, "{{TRACK}}", track);
        t = R(t, "{{DIFFICULTY}}", difficulty);
        t = R(t, "{{CONTEXT}}", context);

        if (questionJson != null) t = R(t, "{{QUESTION_JSON}}", questionJson);
        if (answer != null) t = R(t, "{{ANSWER}}", answer);

        return t;
    }

    private async Task<string> CallOpenRouterOnce(string prompt, string model)
    {
       
        var apiKey = _config["Ai:OpenRouter:ApiKey"]; // OR "Ai:OpenRouter:ApiKey" depending on your JSON
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("OpenRouter:ApiKey is missing/empty at runtime.");

        // var model = _config["OpenRouter:Model"] ?? "openai/gpt-oss-20b:free";
            Console.WriteLine("APIKEY LEN=" + (apiKey?.Length ?? 0));

        var body = new
            {
                model,
                response_format = new { type = "json_object" },
                messages = new[] { new { role = "user", content = prompt } }
            };

        var json = JsonSerializer.Serialize(body);

        var res = await _http.PostAsync(
            "chat/completions",
            new StringContent(json, Encoding.UTF8, "application/json"));

        var raw = await res.Content.ReadAsStringAsync();

        if (!res.IsSuccessStatusCode)
            throw new Exception($"OpenRouter error: {raw}");
        
        if (string.IsNullOrWhiteSpace(raw))
            throw new Exception("OpenRouter returned empty response body (raw).");

        // DEBUG: keep for demo troubleshooting
        Console.WriteLine("OPENROUTER RAW: " + raw);

        using var doc = JsonDocument.Parse(raw);

        var text = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return ExtractJson(text ?? "");
    }

    // ---------------- JSON safety (important for your app) ----------------

    private static string ExtractJson(string text)
    {
        var first = text.IndexOf('{');
        var last = text.LastIndexOf('}');

        if (first >= 0 && last > first)
            return text.Substring(first, last - first + 1);

        return text.Trim();
    }
}
