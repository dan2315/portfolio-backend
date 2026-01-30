using Microsoft.AspNetCore.SignalR;

namespace Portfolio.Infrastructure.Analytics;

public class AnalyticsHub : Hub
{
    public async Task Subscribe()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "analytics-live");
    }

    public async Task Unsubscribe()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "analytics-live");
    }

    public static async Task BroadcastSession(IHubContext<AnalyticsHub> hubContext, object session, CancellationToken token)
    {
        await hubContext.Clients.Group("analytics-live").SendAsync("SessionUpdated", session, token);
    }
}