using Microsoft.Extensions.DependencyInjection;
using Portfolio.Application.Email.Interfaces;

namespace Portfolio.Infrastructure.DependencyInjection;

public static class Email
{
    public static IServiceCollection AddEmail(this IServiceCollection services)
    {
        services.AddSingleton<IEmailService, EmailService>();
        return services;
    }
}
