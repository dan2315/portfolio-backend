using Microsoft.Extensions.DependencyInjection;

namespace Portfolio.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<ILeetCodeService, LeetCodeService>();

        return services;
    }
}