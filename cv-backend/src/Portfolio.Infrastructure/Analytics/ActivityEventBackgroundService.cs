using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Portfolio.Infrastructure.Persistence;

public sealed class ActivityEventBackgroundService : BackgroundService
{
    private readonly Channel<ActivityEvent> _channel;
    private readonly IServiceScopeFactory _scopeFactory;

    public ActivityEventBackgroundService(
        Channel<ActivityEvent> channel,
        IServiceScopeFactory scopeFactory)
    {
        _channel = channel;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var buffer = new List<ActivityEvent>(100);

        await foreach (var evt in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            buffer.Add(evt);

            if (buffer.Count >= 100)
            {
                await FlushAsync(buffer);
                buffer.Clear();
            }
        }
    }

    private async Task FlushAsync(List<ActivityEvent> buffer)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        db.ActivityEvents.AddRange(buffer);
        await db.SaveChangesAsync();
    }
}
