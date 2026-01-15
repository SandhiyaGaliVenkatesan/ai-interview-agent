namespace InterviewAgent.Api.Models;

public class InterviewTurn
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SessionId { get; set; }
    public InterviewSession? Session { get; set; }

    public int TurnNumber { get; set; } // 1..N
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Stored as JSON text for deterministic parsing + UI rendering
    public string QuestionJson { get; set; } = "{}";
    public string UserAnswer { get; set; } = "";
    public string EvaluationJson { get; set; } = "{}";
}
