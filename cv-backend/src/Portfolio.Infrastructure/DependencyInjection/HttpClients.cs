using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Portfolio.Application.Interfaces;
using Portfolio.Infrastructure.GitHub;
using Portfolio.Infrastructure.GitHub.Cache;
using Portfolio.Infrastructure.LeetCode;

namespace Portfolio.Infrastructure.DependencyInjection;

public static class HttpClients
{
    public static IServiceCollection AddHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient<ILeetCodeClient, LeetCodeClient>(client =>
        {
            client.BaseAddress = new Uri("https://leetcode.com/graphql");
        });

        services.AddHttpClient<IGitHubClient, GitHubClient>(ConfigureGitHub);
        services.AddHttpClient<GitHubIds>(ConfigureGitHub);

        services.AddSingleton<ParsedUrlCache<GitHubClient>>();

        return services;
    }

    private static void ConfigureGitHub(IServiceProvider services, HttpClient client)
    {
        var options = services.GetRequiredService<IOptions<GitHubOptions>>().Value;

        client.BaseAddress = new Uri("https://api.github.com/graphql");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", options.Token);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Portfolio-App");
    }
}
