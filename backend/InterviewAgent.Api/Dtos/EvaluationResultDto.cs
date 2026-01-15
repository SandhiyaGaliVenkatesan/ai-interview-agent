namespace InterviewAgent.Api.Dtos;

public record EvaluationResultDto(
    Dictionary<string, int> Scores,
    List<string> Feedback,
    string BetterAnswer,
    string FollowUpQuestion
);
