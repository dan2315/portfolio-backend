using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Data;
using Portfolio.Application.Analytics;
using Portfolio.Application.Analytics.DTOs;
using Portfolio.Application.Analytics.Interfaces;
using Portfolio.Infrastructure.Analytics;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly ILiveSessionsStore _liveSessionsStore;
    private readonly ISessionRepository _sessionRepository;
    private readonly IActivityEventWriter _writer;
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService, IActivityEventWriter writer, ILiveSessionsStore liveSessionsStore, ISessionRepository sessionRepository)
    {
        _writer = writer;
        _liveSessionsStore = liveSessionsStore;
        _sessionRepository = sessionRepository;
        _analyticsService = analyticsService;
    }

    public static void MapExtraRoutes(WebApplication app)
    {
        app.MapHub<AnalyticsHub>("/analytics/live/sessions-sr");
    }

    [HttpPost("event")]
    [RequireAnonIdentity]
    public async Task<ActionResult> ReceiveAnalyticsEvent([FromBody] ActivityEvent activityEvent)
    {
        var anonSession = (Guid) HttpContext.Items[Keys.AnonIdCookieName]!;
        activityEvent.AnonymousId = anonSession;
        await _writer.WriteAsync(activityEvent);
        return Ok(new {message = "ok"});
    }

    [HttpGet("live/sessions")]
    public async Task<ActionResult<IReadOnlyList<SessionDTO>>> SendLiveSnapshot([FromQuery] int limit = 50)
    {
        var sessions = await _liveSessionsStore.GetSessions();
        return Ok(sessions);
    }

    [HttpGet("live/sessions/{id}")]
    public async Task<ActionResult<IReadOnlyList<SessionDTO>>> SendLiveSnapshot(string id, [FromQuery] int hours)
    {

        return Ok(new {message = "ok"});
    }

    [HttpGet("history/sessions")]
    public async Task<ActionResult<IReadOnlyList<SessionDTO>>> SendHistorySnapshot([FromQuery] int hours)
    {
        var sessions = (await _sessionRepository.GetAll()).Select(ActivitySessionMapper.MapToDTO);
        return Ok(sessions);
    }

    [HttpGet("history/sessions/{id}")]
    public async Task<ActionResult<IReadOnlyList<SessionDTO>>> SendHistorySnapshot(string id, [FromQuery] int hours)
    {
        
        return Ok(new {message = "ok"});
    }

    [HttpGet("history/heatmap")]
    public async Task<ActionResult<IReadOnlyList<SessionDTO>>> SendHistoryHeatmap([FromQuery] int year)
    {
        var heatmap = _analyticsService.GetOrComputeSessionsHeatmap(year);
        return Ok(heatmap);
    }
}