using System.Diagnostics;
using Portfolio.Api.Data;

public class ActivityTrackingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IActivityEventWriter _writer;

    public ActivityTrackingMiddleware(RequestDelegate next, IActivityEventWriter writer)
    {
        _next = next;
        _writer = writer;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<TrackActivityAttribute>() is null)
        {
            await _next(context);
            return;
        }

        var sw = Stopwatch.StartNew();
        await _next(context);
        sw.Stop();

        await _writer.WriteAsync(new Portfolio.Application.Analytics.ActivityEvent
        {
            Timestamp = DateTimeOffset.UtcNow,
            EventType = "api_call",
            Route = endpoint.DisplayName!,
            Method = context.Request.Method,
            StatusCode = context.Response.StatusCode,
            DurationMs = sw.ElapsedMilliseconds,
            SessionId = Guid.Parse(context.Request.Cookies[Keys.SessionIdCookieName]??""),
            AnonymousId = context.Items[Keys.AnonIdCookieName] as Guid?,
            UserAgent = context.Request.Headers.UserAgent.ToString()
        });
    }
}
