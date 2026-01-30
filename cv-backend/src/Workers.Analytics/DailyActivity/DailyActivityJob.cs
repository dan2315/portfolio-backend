using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Portfolio.Infrastructure.Analytics;

public class DailyActivityJob(IServiceScopeFactory scopeFactory) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var aggregator = scope.ServiceProvider.GetRequiredService<DailyActivityAggregator>();

            await aggregator.AggregatePendingAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DailyActivityJob failed: {ex}");
        }
    }
}
