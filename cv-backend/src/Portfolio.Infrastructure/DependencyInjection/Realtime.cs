using Microsoft.Extensions.DependencyInjection;
using Portfolio.Infrastructure.Analytics;

namespace Portfolio.Infrastructure.DependencyInjection;

public static class Realtime
{
    public static IServiceCollection AddRealtime(this IServiceCollection services)
    {
        services.AddSignalR();
        services.AddSingleton<LiveSessionsStore>();
        return services;
    }
}
