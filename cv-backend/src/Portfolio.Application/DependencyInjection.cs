using Microsoft.Extensions.DependencyInjection;
using Portfolio.Application.Messages;
using Portfolio.Application.Messages.Interfaces;
using Portfolio.Application.Projects;
using Portfolio.Application.Projects.Interfaces;

namespace Portfolio.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<ILeetCodeService, LeetCodeService>();
        services.AddScoped<IProjectsService, ProjectsService>();
        services.AddScoped<IMessagesService, MessagesService>();

        return services;
    }
}