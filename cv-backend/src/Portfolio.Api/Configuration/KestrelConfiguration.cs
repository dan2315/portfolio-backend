using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Portfolio.Api.Configuration;

public static class KestrelConfiguration
{
    public static void ConfigureKestrelForDebug(this WebApplicationBuilder builder)
    {
        var certPath = "/certs/dev.pfx";
        var useCerts = File.Exists(certPath);

        Console.WriteLine("Use certs: " + useCerts);

        builder.WebHost.ConfigureKestrel(options =>
        {
            if (useCerts)
            {
                options.ListenAnyIP(8081, lo =>
                {
                    lo.Protocols = HttpProtocols.Http2;
                });
                options.ListenAnyIP(8080);
            }
        });
    }
}