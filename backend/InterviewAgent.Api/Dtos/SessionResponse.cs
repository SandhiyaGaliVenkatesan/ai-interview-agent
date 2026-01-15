namespace InterviewAgent.Api.Dtos;

public record SessionResponse(
    Guid Id,
    string Track,
    string Difficulty,
    DateTime CreatedAtUtc,
    int TurnsCount
);
