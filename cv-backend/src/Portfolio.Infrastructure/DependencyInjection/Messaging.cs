using System.Threading.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Application.Analytics;
using Portfolio.Infrastructure.Analytics;
using Portfolio.Infrastructure.RabbitMQ;
using RabbitMQ.Client;

namespace Portfolio.Infrastructure.DependencyInjection;

public static class Messaging
{
    public static IServiceCollection AddMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton(Channel.CreateUnbounded<ActivityEvent>());

        services.AddSingleton<IConnectionFactory>(_ =>
            new ConnectionFactory
            {
                Uri = new Uri(configuration["RabbitMq:Uri"]
                    ?? throw new Exception("RabbitMq:Uri is not present"))
            });

        services.AddSingleton<RmqConnectionHolder>();
        services.AddHostedService<RmqInitializer>();
        services.AddSingleton<IActivityEventWriter, RmqActivityPublisher>();

        return services;
    }
}
