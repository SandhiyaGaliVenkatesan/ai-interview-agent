namespace InterviewAgent.Api.Models;

public class InterviewSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Track { get; set; } = "dotnet"; // dotnet | react | system-design
    public string Difficulty { get; set; } = "medium"; // easy | medium | hard
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public List<InterviewTurn> Turns { get; set; } = new();
}
