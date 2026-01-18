using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace Portfolio.Infrastructure.RabbitMQ;

public sealed class RmqInitializer : IHostedService
{
    private readonly RmqConnectionHolder _connection;
    private readonly IConnectionFactory _factory;

    public RmqInitializer(
        RmqConnectionHolder connection,
        IConnectionFactory factory)
    {
        _connection = connection;
        _factory = factory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var connection = await _factory.CreateConnectionAsync(cancellationToken);
        await _connection.Populate(connection);
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}