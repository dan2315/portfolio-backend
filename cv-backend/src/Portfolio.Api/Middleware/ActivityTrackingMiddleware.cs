using System.Diagnostics;

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

        await _writer.WriteAsync(new ActivityEvent
        {
            Timestamp = DateTimeOffset.UtcNow,
            EventType = "api_call",
            Route = endpoint.DisplayName!,
            Method = context.Request.Method,
            StatusCode = context.Response.StatusCode,
            DurationMs = sw.ElapsedMilliseconds,
            AnonymousSessionId = context.Items["AnonymousSessionId"] as Guid?,
            UserId = context.User.Identity?.IsAuthenticated == true
                ? context.User.FindFirst("sub")?.Value
                : null,
            UserAgent = context.Request.Headers.UserAgent.ToString()
        });
    }
}
