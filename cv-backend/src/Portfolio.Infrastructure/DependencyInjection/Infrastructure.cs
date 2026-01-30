using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Infrastructure.Analytics.InterappTransport;

namespace Portfolio.Infrastructure.DependencyInjection;

public static class Infrastructure
{
    public static IServiceCollection AddWebInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddCoreInfrastructure(configuration)
            .AddMessaging(configuration)
            .AddRealtime()
            .AddMemoryCache();
    }

    public static IServiceCollection AddWorkerInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddCoreInfrastructure(configuration)
            .AddMessaging(configuration)
            .AddRealtime()
            .AddSingleton<SessionDeltaGrpcClient>();
    }

    public static IServiceCollection AddMigratorInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddDatabase(configuration);
    }


    private static IServiceCollection AddCoreInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddDatabase(configuration)
            .AddHttpClients()
            .AddRepositories()
            .AddEmail()
            .AddCache(configuration);
    }
}
