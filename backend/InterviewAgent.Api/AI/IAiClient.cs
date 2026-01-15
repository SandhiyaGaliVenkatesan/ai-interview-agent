namespace InterviewAgent.Api.Services.Ai;

public interface IAiClient
{
    Task<string> GenerateQuestionJsonAsync(string track, string difficulty, string? context);
    Task<string> EvaluateAnswerJsonAsync(string track, string difficulty, string questionJson, string userAnswer);
}
