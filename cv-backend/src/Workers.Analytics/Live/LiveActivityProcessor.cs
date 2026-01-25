using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Portfolio.Application.Analytics;
using Portfolio.Infrastructure.Analytics;
using Portfolio.Infrastructure.Analytics.Interfaces;
using Portfolio.Infrastructure.RabbitMQ;
using RabbitMQ.Client.Events;
using StackExchange.Redis;

namespace Workers.Analytics.Live;

public sealed class LiveAnalyticsProcessor : BackgroundService
{
    private readonly RmqConnectionHolder _rmq;
    private readonly ILiveSessionsStore _sessionsStore;
    private readonly ConcurrentDictionary<Guid, (SessionDeltaState sessionDelta, List<ulong> deliveryTags)> _buffer = new();
    private readonly SemaphoreSlim _flushLock = new(1, 1);

    public LiveAnalyticsProcessor(RmqConnectionHolder rmq, ILiveSessionsStore sessionsStore)
    {
        _rmq = rmq;
        _sessionsStore = sessionsStore;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = _rmq.Channel;

        await channel.BasicQosAsync(0, prefetchCount: 500, global: false, cancellationToken: stoppingToken);
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += OnMessageAsync;

        await channel.BasicConsumeAsync(
            queue: "analytics.activity-live",
            autoAck: false,
            consumerTag: "",
            noLocal: false,
            exclusive: false,
            arguments: null,
            consumer: consumer,
            cancellationToken: stoppingToken
        );

        _ = FlushLoop(stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task OnMessageAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        try
        {
            var json = Encoding.UTF8.GetString(eventArgs.Body.Span);
            var activityEvent = JsonSerializer.Deserialize<ActivityEvent>(json) ?? throw new Exception("Failed to deserialize ActivityEvent");

            var session = new SessionDeltaState(activityEvent);
            _buffer.AddOrUpdate(
                activityEvent.SessionId!.Value,
                _ => new(session, [eventArgs.DeliveryTag]),
                (_, existing) =>
                {
                    existing.sessionDelta.ApplyEvent(activityEvent);
                    existing.deliveryTags.Add(eventArgs.DeliveryTag);
                    return existing;
                }
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to process activity event: " + ex.ToString());

            await _rmq.Channel.BasicNackAsync(
                eventArgs.DeliveryTag,
                multiple: false,
                requeue: false
            );
        }
    }

    private async Task FlushLoop(CancellationToken token)
    {
        var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));

        while (await timer.WaitForNextTickAsync(token))
        {
            if (!await _flushLock.WaitAsync(0, token))
                continue;

            try
            {
                foreach (var (sessionId, buffered) in _buffer)
                {
                    if (!_buffer.TryRemove(sessionId, out var entry))
                        continue;

                    try
                    {
                        await _sessionsStore.StoreSessions(entry.sessionDelta);
                        
                        foreach (var tag in entry.deliveryTags)
                        {
                            Console.WriteLine($"Message with tag {tag} was processed");
                            await _rmq.Channel.BasicAckAsync(tag, multiple: false);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);

                        foreach (var tag in entry.deliveryTags)
                        {
                            await _rmq.Channel.BasicNackAsync(
                                tag,
                                multiple: false,
                                requeue: true
                            );
                        }
                    }
                }
            }
            finally
            {
                _flushLock.Release();
            }
        }
    }
}