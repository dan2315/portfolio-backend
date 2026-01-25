using Microsoft.Extensions.DependencyInjection;
using Portfolio.Application.Messages.Interfaces;
using Portfolio.Application.Projects.Interfaces;
using Portfolio.Infrastructure.Analytics;
using Portfolio.Infrastructure.Analytics.Interfaces;
using Portfolio.Infrastructure.Messages;

namespace Portfolio.Infrastructure.DependencyInjection;

public static class Repositories
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProjectsRepository, ProjectsRepository>();
        services.AddScoped<IMessagesRepository, MessagesRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddSingleton<ILiveSessionsStore, LiveSessionsStore>();

        return services;
    }
}
