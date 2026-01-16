using InterviewAgent.Api.Dtos;
using InterviewAgent.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace InterviewAgent.Api.Controllers;

[ApiController]
[Route("api/sessions")]
public class SessionsController : ControllerBase
{
    private readonly InterviewService _svc;

    public SessionsController(InterviewService svc)
    {
        _svc = svc;
    }

    [HttpPost]
    public async Task<ActionResult<SessionResponse>> Create([FromBody] CreateSessionRequest req)
    {
        var session = await _svc.CreateSessionAsync(req.Track, req.Difficulty);
        return Ok(new SessionResponse(session.Id, session.Track, session.Difficulty, session.CreatedAtUtc, 0));
    }

    [HttpGet]
    public async Task<ActionResult<List<SessionResponse>>> List()
        => Ok(await _svc.ListSessionsAsync());

    [HttpGet("{sessionId:guid}")]
    public async Task<ActionResult<object>> Get(Guid sessionId)
    {
        var session = await _svc.GetSessionAsync(sessionId);
        return Ok(new
        {
            session.Id,
            session.Track,
            session.Difficulty,
            session.CreatedAtUtc,
            turns = session.Turns.Select(t => new
            {
                t.TurnNumber,
                t.CreatedAtUtc,
                t.QuestionJson,
                t.UserAnswer,
                t.EvaluationJson
            })
        });
    }

    [HttpPost("{sessionId:guid}/question")]
    public async Task<ActionResult<object>> GenerateQuestion(Guid sessionId, [FromBody] GenerateQuestionRequest req)
    {
        var turn = await _svc.GenerateNextQuestionAsync(sessionId, req.Context);
        return Ok(new { turn.TurnNumber, turn.QuestionJson });
    }

    [HttpPost("{sessionId:guid}/turns/{turnNumber:int}/answer")]
    public async Task<ActionResult<object>> SubmitAnswer(Guid sessionId, int turnNumber, [FromBody] SubmitAnswerRequest req)
    {
        var turn = await _svc.SubmitAnswerAsync(sessionId, turnNumber, req.Answer);
        using var doc = JsonDocument.Parse(turn.EvaluationJson ?? "{}");
        var root = doc.RootElement;

        var dto = new EvaluationDto(
            root.GetProperty("score").GetInt32(),
            root.GetProperty("verdict").GetString() ?? "Needs Improvement",
            root.GetProperty("strengths").EnumerateArray().Select(x => x.GetString() ?? "").ToList(),
            root.GetProperty("improvements").EnumerateArray().Select(x => x.GetString() ?? "").ToList(),
            root.GetProperty("modelAnswer").GetString() ?? ""
        );
        return Ok(new { turn.TurnNumber, dto });
    }
}
