using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Portfolio.Infrastructure.RabbitMQ;

public class RmqConnectionHolder : IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;
    public IConnection Connection => _connection ?? throw new InvalidOperationException("RabbitMQ connection not initialized");
    public IChannel Channel => _channel ?? throw new InvalidOperationException("RabbitMQ channel not initialized");
    private bool _disposed = false;
    
    public async Task Populate(IConnection connection)
    {
        _connection = connection;
        _channel = await _connection.CreateChannelAsync();
        
        await _channel.ExchangeDeclareAsync(
            exchange: "analytics",
            type: ExchangeType.Topic,
            durable: true
        );

        await _channel.QueueDeclareAsync(
            queue: "analytics.activity",
            durable: true,
            exclusive: false,
            autoDelete: false
        );

        await _channel.QueueBindAsync(
            queue: "analytics.activity",
            exchange: "analytics",
            routingKey: "activity.*"
        );
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        await Connection.CloseAsync();
        await Channel.CloseAsync();
        await Connection.DisposeAsync();
        await Channel.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}