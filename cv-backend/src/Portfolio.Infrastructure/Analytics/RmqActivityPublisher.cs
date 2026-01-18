using System.Text;
using System.Text.Json;
using Portfolio.Infrastructure.RabbitMQ;
using RabbitMQ.Client;

namespace Portfolio.Infrastructure.Analytics;

public class RmqActivityPublisher : IActivityEventWriter, IAsyncDisposable
{
    private const string ExchangeName = "analytics";
    private const string RoutingKey = "activity.page_view";

    private readonly RmqConnectionHolder _connectionHolder;
    private readonly BasicProperties _basicProperties;

    public RmqActivityPublisher(RmqConnectionHolder connectionHolder)
    {
        _connectionHolder = connectionHolder;
        _basicProperties = new BasicProperties();
        _basicProperties.ContentType = "text/plain";
        _basicProperties.DeliveryMode = DeliveryModes.Persistent;
    }

    public async ValueTask WriteAsync(Application.Analytics.ActivityEvent activityEvent)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(activityEvent));

        await _connectionHolder.Channel.BasicPublishAsync(
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