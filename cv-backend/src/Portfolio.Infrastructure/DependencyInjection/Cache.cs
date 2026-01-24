using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Portfolio.Infrastructure.DependencyInjection;

public static class Cache
{
    public static IServiceCollection AddCache(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "portfolio:";
        });

        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var options = ConfigurationOptions.Parse(
                configuration.GetConnectionString("Redis")!);

            options.AbortOnConnectFail = false;
            return ConnectionMultiplexer.Connect(options);
        });

        return services;
    }
}
