using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Portfolio.Application.Analytics;
using Portfolio.Infrastructure.Persistence;
using Portfolio.Infrastructure.RabbitMQ;
using RabbitMQ.Client.Events;

namespace Workers.Analytics.History;

public sealed class HistoryActivityProcessor : BackgroundService
{
    private readonly ConcurrentDictionary<Guid, SessionDeltaState> _recentSessions = new();
    private readonly RmqConnectionHolder _rmq;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ConcurrentDictionary<Guid, (SessionDeltaState State, ConcurrentBag<ulong> deliveryTags, DateTimeOffset LastSeen)> _activeSessions = new();
    private const int MaxSessionsBeforeFlush = 1000;
    private readonly SemaphoreSlim _flushLock = new(1, 1);

    public HistoryActivityProcessor(IServiceScopeFactory scopeFactory, RmqConnectionHolder rmq)
    {
        _scopeFactory = scopeFactory;
        _rmq = rmq;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = _rmq.Channel;

        await channel.BasicQosAsync(0, prefetchCount: 500, global: false, cancellationToken: stoppingToken);
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += OnMessageAsync;

        await channel.BasicConsumeAsync(
            queue: "analytics.activity-history",
            autoAck: false,
            consumerTag: "",
            noLocal: false,
            exclusive: false,
            arguments: null,
            consumer: consumer,
            cancellationToken: stoppingToken
        );

        _ = PostgresFlushLoop(stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private Task OnMessageAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        var json = Encoding.UTF8.GetString(eventArgs.Body.Span);
        var activityEvent = JsonSerializer.Deserialize<ActivityEvent>(json)
            ?? throw new Exception("Failed to deserialize ActivityEvent");

        var now = activityEvent.Timestamp;
        Console.WriteLine($"Recieved message: {eventArgs.DeliveryTag}");
        _activeSessions.AddOrUpdate(
            activityEvent.SessionId!.Value,
            _ => (new SessionDeltaState(activityEvent), [eventArgs.DeliveryTag], now),
            (_, existing) =>
            {
                existing.State.ApplyEvent(activityEvent);
                existing.deliveryTags.Add(eventArgs.DeliveryTag);
                existing.LastSeen = now;
                return existing;
            });

        if (ShouldForceFlush())
        {
            var sessionsToFlush = _activeSessions.ToList();
            _ = Task.Run(() => TryFlushAsync(sessionsToFlush, CancellationToken.None));
        }

        return Task.CompletedTask;
    }

    private async Task PostgresFlushLoop(CancellationToken token)
    {
        // var timer = new PeriodicTimer(TimeSpan.FromMinutes(10));
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

        try
        {
            while (await timer.WaitForNextTickAsync(token))
            {
                var cutoff = DateTimeOffset.UtcNow.AddMinutes(0);
                // var cutoff = DateTimeOffset.UtcNow.AddMinutes(-5);
                var sessionsToFlush = _activeSessions
                    .Where(kvp => kvp.Value.LastSeen < cutoff)
                    .ToList();

                if (sessionsToFlush.Count == 0)
                    continue;


                Console.WriteLine($"Making attempt to flush {sessionsToFlush} sessions" );
                await TryFlushAsync(sessionsToFlush, token);
            }    
        }
        catch (OperationCanceledException)
        {
            await TryFlushAsync(_activeSessions.ToList(), CancellationToken.None);
            throw;
        }
        
    }
    private bool ShouldForceFlush() =>
        _activeSessions.Count >= MaxSessionsBeforeFlush;


    private async Task TryFlushAsync(List<KeyValuePair<Guid, (SessionDeltaState State, ConcurrentBag<ulong> deliveryTags, DateTimeOffset LastSeen)>> sessionsToFlush, CancellationToken token)
    {
        if (sessionsToFlush.Count == 0)
            return;

        if (!await _flushLock.WaitAsync(0, token))
        {
            return;
        }

        try
        {
            await _FlushAsync(sessionsToFlush, token);
        }
        finally
        {
            _flushLock.Release();
        }
    }

    private async Task _FlushAsync(List<KeyValuePair<Guid, (SessionDeltaState State, ConcurrentBag<ulong> deliveryTags, DateTimeOffset LastSeen)>> sessionsToFlush, CancellationToken token)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var values = new List<string>();
        var parameters = new List<object>();
        int i = 0;

        foreach (var (sessionId, entry) in sessionsToFlush)
        {
            values.Add($"(@p{i*7}, @p{i*7+1}, @p{i*7+2}, @p{i*7+3}, @p{i*7+4}, @p{i*7+5}, @p{i*7+6}, @p{i*7+7})");
            parameters.Add(entry.State.SessionId);
            parameters.Add(entry.State.AnonymousId);
            parameters.Add(entry.State.LeastStartTime);
            parameters.Add(entry.State.GreatestEndTime);
            parameters.Add(entry.State.PagesViewed);
            parameters.Add(entry.State.CartridgesInserted);
            parameters.Add(entry.State.ContactAttempts);
            parameters.Add(entry.State.TotalTimeMs);

            i++;
        }

        var sql = UpsertSessionsQuerry(values);

        try
        {
            Console.WriteLine($"Message with tag {sessionsToFlush.Count} was processed");
            await using var transaction = await dbContext.Database.BeginTransactionAsync(token);
            await dbContext.Database.ExecuteSqlRawAsync(sql, parameters.ToArray(), token);
            await transaction.CommitAsync(token);

            foreach (var (sessionId, entry) in sessionsToFlush)
            {
                foreach (var tag in entry.deliveryTags)
                {
                    await _rmq.Channel.BasicAckAsync(tag, multiple: false, cancellationToken: token);
                }

                _activeSessions.TryRemove(sessionId, out _);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to flush sessions: {ex}");
        }
    }

    private static string UpsertSessionsQuerry(List<string> values)
    {
        return $"""
        INSERT INTO activity_sessions (
            "SessionId",
            "AnonymousId",
            "StartTime",
            "EndTime",
            "PagesViewed",
            "CartridgesInserted",
            "ContactAttempted",
            "TotalTimeMs"
        )
        VALUES {string.Join(", ", values)}
        ON CONFLICT ("SessionId") DO UPDATE SET
            "StartTime" = LEAST(activity_sessions."StartTime", EXCLUDED."StartTime"),
            "EndTime"   = GREATEST(activity_sessions."EndTime", EXCLUDED."EndTime"),
            "PagesViewed" = activity_sessions."PagesViewed" + EXCLUDED."PagesViewed",
            "CartridgesInserted" = activity_sessions."CartridgesInserted" + EXCLUDED."CartridgesInserted",
            "ContactAttempted" = activity_sessions."ContactAttempted" + EXCLUDED."ContactAttempted",
            "TotalTimeMs" =
                EXTRACT(EPOCH FROM (
                    GREATEST(activity_sessions."EndTime", EXCLUDED."EndTime")
                - LEAST(activity_sessions."StartTime", EXCLUDED."StartTime")
                )) * 1000;
        """;
    }
}