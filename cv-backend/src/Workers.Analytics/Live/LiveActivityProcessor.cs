using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Portfolio.Application.Analytics;
using Portfolio.Application.Analytics.Interfaces;
using Portfolio.Infrastructure.Analytics.InterappTransport;
using Portfolio.Infrastructure.RabbitMQ;
using RabbitMQ.Client.Events;

namespace Workers.Analytics.Live;

public sealed class LiveAnalyticsProcessor : BackgroundService
{
    private readonly RmqConnectionHolder _rmq;
    private readonly ILiveSessionsStore _sessionsStore;
    private readonly SessionDeltaGrpcClient _grpcClient;
    private readonly ConcurrentDictionary<Guid, BufferedSession> _buffer = new();
    private readonly SemaphoreSlim _flushLock = new(1, 1);

    public LiveAnalyticsProcessor(RmqConnectionHolder rmq, ILiveSessionsStore sessionsStore, SessionDeltaGrpcClient grpcClient)
    {
        _rmq = rmq;
        _sessionsStore = sessionsStore;
        _grpcClient = grpcClient;
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

            _buffer.AddOrUpdate(
                activityEvent.SessionId!.Value,
                _ => new BufferedSession(new SessionDeltaState(activityEvent), [eventArgs.DeliveryTag]),
                (_, existing) =>
                {
                    existing.SessionDelta.ApplyEvent(activityEvent);
                    existing.DeliveryTags.Add(eventArgs.DeliveryTag);
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
                    try
                    {
                        if (buffered.DeliveryTags.IsEmpty) continue;

                        await _grpcClient.PublishSessionDeltaAsync(buffered.SessionDelta);
                        await _sessionsStore.StoreSessions(buffered.SessionDelta);

                        foreach (var tag in buffered.DeliveryTags)
                        {
                            Console.WriteLine($"Message with tag {tag} was processed");
                            await _rmq.Channel.BasicAckAsync(tag, multiple: false);
                        }
                        
                        buffered.SessionDelta.Reset();
                        buffered.DeliveryTags.Clear();

                        if (buffered.SessionDelta.SessionExpiresAt <= DateTimeOffset.UtcNow)
                        {
                            _buffer.TryRemove(sessionId, out _);
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);

                        foreach (var tag in buffered.DeliveryTags)
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

sealed class BufferedSession
{
    public SessionDeltaState SessionDelta { get; }
    public ConcurrentBag<ulong> DeliveryTags { get; private set; }

    public BufferedSession(SessionDeltaState delta, ConcurrentBag<ulong> deliveryTags)
    {
        SessionDelta = delta;
        DeliveryTags = deliveryTags;
    }
}