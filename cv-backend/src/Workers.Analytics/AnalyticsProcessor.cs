using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Portfolio.Application.Analytics;
using Portfolio.Infrastructure.Persistence;
using Portfolio.Infrastructure.RabbitMQ;
using RabbitMQ.Client.Events;

namespace Workers.Analytics;

public sealed class AnalyticsProcessor : BackgroundService
{
    private readonly RmqConnectionHolder _rmq;
    private readonly IServiceScopeFactory _scopeFactory;

    public AnalyticsProcessor(IServiceScopeFactory scopeFactory, RmqConnectionHolder rmq)
    {
        _scopeFactory = scopeFactory;
        _rmq = rmq;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = _rmq.Channel;

        await channel.BasicQosAsync(0, prefetchCount: 1, global: false, cancellationToken: stoppingToken);
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += OnMessageAsync;

        await channel.BasicConsumeAsync(
            queue: "analytics.activity",
            autoAck: false,
            consumerTag: "",
            noLocal: false,
            exclusive: false,
            arguments: null,
            consumer: consumer,
            cancellationToken: stoppingToken
        );

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task OnMessageAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        try
        {
            var json = Encoding.UTF8.GetString(eventArgs.Body.Span);
            var activityEvent = JsonSerializer.Deserialize<ActivityEvent>(json) ?? throw new Exception("asdfg");

            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbContext.ActivityEvents.Add(activityEvent);
            dbContext.SaveChanges();

            await _rmq.Channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
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
}