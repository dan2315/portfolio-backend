using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Portfolio.Api.Configuration;

public static class AppConfigurationExtensions
{
    public static void AddApplicationVariables(this IConfigurationBuilder manager, IWebHostEnvironment environment)
    {
        manager
        .AddJsonFile("appsettings.json", optional: true)
        .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
        .AddUserSecrets<Program>(optional: true)
        .AddEnvironmentVariables();
    }

    public static string[] GetAllowedOrigins(this IWebHostEnvironment env)
    {
        return env.IsDevelopment()
            ? ["http://localhost:3000"]
            :
            [
                "https://interactive-porfolio.vercel.app",
                "https://portfolio.danielthedod.dev",
                "https://www.portfolio.danielthedod.dev"
            ];
    }
}