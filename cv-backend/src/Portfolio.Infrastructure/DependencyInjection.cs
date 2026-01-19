using System.Net.Http.Headers;
using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Portfolio.Application.Email.Interfaces;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Messages.Interfaces;
using Portfolio.Application.Projects.Interfaces;
using Portfolio.Infrastructure.Analytics;
using Portfolio.Infrastructure.GitHub;
using Portfolio.Infrastructure.GitHub.Cache;
using Portfolio.Infrastructure.LeetCode;
using Portfolio.Infrastructure.Messages;
using Portfolio.Infrastructure.Persistence;
using Portfolio.Infrastructure.RabbitMQ;
using RabbitMQ.Client;

namespace Portfolio.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<ILeetCodeClient, LeetCodeClient>(client =>
        {
            client.BaseAddress = new Uri("https://leetcode.com/graphql");
        });

        services.AddHttpClient<IGitHubClient, GitHubClient>(ConfigureGitHubHttpClient);
        services.AddHttpClient<GitHubIds>(ConfigureGitHubHttpClient);
        services.AddSingleton<ParsedUrlCache<GitHubClient>>();

        services.AddDbContext<AppDbContext>(options =>
        {
            Console.WriteLine("Postgres connection string: " + configuration.GetConnectionString("Postgres"));
            options.UseNpgsql(configuration.GetConnectionString("Postgres"));
        });

        services.AddSingleton<IEmailService, EmailService>();

        Channel<Application.Analytics.ActivityEvent> channel = Channel.CreateUnbounded<Application.Analytics.ActivityEvent>();
        services.AddSingleton(channel);

        services.AddSingleton<IConnectionFactory>(_ => 
            new ConnectionFactory
            {
                Uri = new Uri(configuration["RabbitMq:Uri"] ?? throw new Exception("RabbitMq:Uri is not present"))
            }
        );
        services.AddHostedService<RmqInitializer>();
        services.AddSingleton<RmqConnectionHolder>();
        services.AddSingleton<IActivityEventWriter, RmqActivityPublisher>();

        services.AddScoped<IProjectsRepository, ProjectsRepository>();
        services.AddScoped<IMessagesRepository, MessagesRepository>();
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