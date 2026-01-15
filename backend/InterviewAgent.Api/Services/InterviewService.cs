using System.Text.Json;
using InterviewAgent.Api.Data;
using InterviewAgent.Api.Dtos;
using InterviewAgent.Api.Models;
using InterviewAgent.Api.Services.Ai;
using Microsoft.EntityFrameworkCore;

namespace InterviewAgent.Api.Services;

public class InterviewService
{
    private readonly AppDbContext _db;
    private readonly IAiClient _ai;

    public InterviewService(AppDbContext db, IAiClient ai)
    {
        _db = db;
        _ai = ai;
    }

    public async Task<InterviewSession> CreateSessionAsync(string track, string difficulty)
    {
        track = NormalizeTrack(track);
        difficulty = NormalizeDifficulty(difficulty);

        var session = new InterviewSession { Track = track, Difficulty = difficulty };
        _db.Sessions.Add(session);
        await _db.SaveChangesAsync();
        return session;
    }

    public async Task<InterviewTurn> GenerateNextQuestionAsync(Guid sessionId, string? context)
    {
        var session = await _db.Sessions.Include(s => s.Turns).FirstOrDefaultAsync(s => s.Id == sessionId)
                      ?? throw new KeyNotFoundException("Session not found");

        var nextTurnNumber = session.Turns.Count + 1;

        var questionJson = await _ai.GenerateQuestionJsonAsync(session.Track, session.Difficulty, context);

        // Minimal validation: must parse as JSON
        JsonDocument.Parse(questionJson);

        var turn = new InterviewTurn
        {
            SessionId = session.Id,
            TurnNumber = nextTurnNumber,
            QuestionJson = questionJson
        };

        _db.Turns.Add(turn);
        await _db.SaveChangesAsync();
        return turn;
    }

    public async Task<InterviewTurn> SubmitAnswerAsync(Guid sessionId, int turnNumber, string answer)
    {
        var turn = await _db.Turns.FirstOrDefaultAsync(t => t.SessionId == sessionId && t.TurnNumber == turnNumber)
                   ?? throw new KeyNotFoundException("Turn not found");

        var session = await _db.Sessions.FirstAsync(s => s.Id == sessionId);

        turn.UserAnswer = answer ?? "";

        var evalJson = await _ai.EvaluateAnswerJsonAsync(session.Track, session.Difficulty, turn.QuestionJson, turn.UserAnswer);

        JsonDocument.Parse(evalJson);

        turn.EvaluationJson = evalJson;

        await _db.SaveChangesAsync();
        return turn;
    }

    public async Task<List<SessionResponse>> ListSessionsAsync()
    {
        return await _db.Sessions
            .OrderByDescending(s => s.CreatedAtUtc)
            .Select(s => new SessionResponse(s.Id, s.Track, s.Difficulty, s.CreatedAtUtc, s.Turns.Count))
            .ToListAsync();
    }

    public async Task<InterviewSession> GetSessionAsync(Guid sessionId)
    {
        return await _db.Sessions.Include(s => s.Turns.OrderBy(t => t.TurnNumber))
            .FirstOrDefaultAsync(s => s.Id == sessionId)
            ?? throw new KeyNotFoundException("Session not found");
    }

    private static string NormalizeTrack(string track)
    {
        track = (track ?? "").Trim().ToLowerInvariant();
        return track switch
        {
            "react" => "react",
            "system-design" or "systemdesign" or "sd" => "system-design",
            _ => "dotnet"
        };
    }

    private static string NormalizeDifficulty(string difficulty)
    {
        difficulty = (difficulty ?? "").Trim().ToLowerInvariant();
        return difficulty switch
        {
            "easy" => "easy",
            "hard" => "hard",
            _ => "medium"
        };
    }
}
