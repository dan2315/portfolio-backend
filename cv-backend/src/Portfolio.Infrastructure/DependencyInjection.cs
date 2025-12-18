using Microsoft.Extensions.DependencyInjection;
using Portfolio.Application.Interfaces;
using Portfolio.Infrastructure.LeetCode;

namespace Portfolio.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastruture(this IServiceCollection services)
    {
        services.AddHttpClient<ILeetCodeClient, LeetCodeClient>(client =>
        {
            client.BaseAddress = new Uri("https://leetcode.com/graphql");
        });

        return services;
    }
}