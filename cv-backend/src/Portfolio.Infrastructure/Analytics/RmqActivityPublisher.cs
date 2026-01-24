using System.Text;
using System.Text.Json;
using Portfolio.Infrastructure.RabbitMQ;
using RabbitMQ.Client;

namespace Portfolio.Infrastructure.Analytics;

public class RmqActivityPublisher : IActivityEventWriter, IAsyncDisposable
{
    private const string ExchangeName = "analytics";
    private const string RoutingKey = "activity.event";

    private readonly RmqConnectionHolder _connectionHolder;
    private readonly BasicProperties _basicProperties;

    public RmqActivityPublisher(RmqConnectionHolder connectionHolder)
    {
        _connectionHolder = connectionHolder;
        _basicProperties = new BasicProperties
        {
            ContentType = "text/plain",
            DeliveryMode = DeliveryModes.Persistent
        };
    }

    public async ValueTask WriteAsync(Application.Analytics.ActivityEvent activityEvent)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(activityEvent));

        //consumer channel → consume only
        // ack channel → ACK only
        await _connectionHolder.Channel.BasicPublishAsync( // TODO: Reason number of used channels
            exchange: ExchangeName,
            routingKey: RoutingKey,
            mandatory: false,
            basicProperties: _basicProperties,
            body: body 
        );
    }

    public async ValueTask DisposeAsync()
    {
        await _connectionHolder.DisposeAsync();
    }
}