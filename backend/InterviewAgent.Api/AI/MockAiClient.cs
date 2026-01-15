using System.Text.Json;

namespace InterviewAgent.Api.Services.Ai;

public class MockAiClient : IAiClient
{
    public Task<string> GenerateQuestionJsonAsync(string track, string difficulty, string? context)
    {
        var obj = new
        {
            question = track switch
            {
                "react" => "Explain how React reconciliation works and why keys matter.",
                "system-design" => "Design a URL shortener. What are the key components and tradeoffs?",
                _ => "Explain dependency injection in ASP.NET Core and why it improves testability."
            },
            difficulty,
            tags = new[] { track, "core" },
            expectedAnswerHints = new[]
            {
                "Define concept clearly",
                "Give a real example",
                "Mention tradeoffs"
            }
        };

        return Task.FromResult(JsonSerializer.Serialize(obj));
    }

    public Task<string> EvaluateAnswerJsonAsync(string track, string difficulty, string questionJson, string userAnswer)
    {
        // Simple heuristic scoring for demo:
        int score(string keyword) => userAnswer.Contains(keyword, StringComparison.OrdinalIgnoreCase) ? 4 : 2;

        var scores = new Dictionary<string, int>
        {
            ["clarity"] = Math.Clamp(score("because"), 1, 5),
            ["correctness"] = Math.Clamp(score("example"), 1, 5),
            ["depth"] = Math.Clamp(score("tradeoff"), 1, 5),
            ["examples"] = Math.Clamp(score("for instance"), 1, 5)
        };

        var obj = new
        {
            scores,
            feedback = new[]
            {
                "Add a clearer definition first, then an example.",
                "Mention at least one tradeoff or edge case."
            },
            betterAnswer = "A stronger answer would define the concept, explain the mechanism, then give a real example and tradeoffs.",
            followUpQuestion = "Can you explain a tradeoff or edge case related to your answer?"
        };

        return Task.FromResult(JsonSerializer.Serialize(obj));
    }
}
