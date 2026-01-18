using Microsoft.Extensions.Configuration;

namespace Workers.Analytics.Configuration;

public static class ConfigurationExtensions
{
    public static void AddWorkerVariables(this IConfigurationBuilder builder)
    {
        builder
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>(optional: true);
    }
}