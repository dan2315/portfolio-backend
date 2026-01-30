using Microsoft.Extensions.DependencyInjection;
using Portfolio.Infrastructure.Analytics;

namespace Portfolio.Infrastructure.DependencyInjection;

public static class Realtime
{
    public static IServiceCollection AddRealtime(this IServiceCollection services)
    {
        services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = true;
        });
        services.AddSignalR();
        return services;
    }
}
