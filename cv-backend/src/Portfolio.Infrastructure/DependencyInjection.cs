using System.Net.Http.Headers;
using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Projects.Interfaces;
using Portfolio.Infrastructure.GitHub;
using Portfolio.Infrastructure.GitHub.Cache;
using Portfolio.Infrastructure.LeetCode;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastruture(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<ILeetCodeClient, LeetCodeClient>(client =>
        {
            client.BaseAddress = new Uri("https://leetcode.com/graphql");
        });

        services.AddHttpClient<IGitHubClient, GitHubClient>(ConfigureGitHubHttpClient);
        services.AddHttpClient<GitHubIds>(ConfigureGitHubHttpClient);

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Postgres"));
        });

        Channel<ActivityEvent> channel = Channel.CreateUnbounded<ActivityEvent>();
        services.AddSingleton(channel);
        services.AddSingleton<ParsedUrlCache<GitHubClient>>();

        services.AddSingleton<IActivityEventWriter, ActivityEventWriter>();
        services.AddScoped<IProjectsRepository, ProjectsRepository>();
        services.AddHostedService<ActivityEventBackgroundService>();
        services.AddMemoryCache();

        return services;
    }

    private static void ConfigureGitHubHttpClient(IServiceProvider services, HttpClient client)
    {
        var options = services.GetRequiredService<IOptions<GitHubOptions>>().Value;
        client.BaseAddress = new Uri("https://api.github.com/graphql");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.Token);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Portfolio-App");
    }
}