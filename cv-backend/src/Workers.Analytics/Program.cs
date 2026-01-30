using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Portfolio.Infrastructure.Analytics;
using Portfolio.Infrastructure.DependencyInjection;
using Workers.Analytics;
using Workers.Analytics.Configuration;
using Workers.Analytics.History;
using Workers.Analytics.Live;
using Workers.Analytics.MockEventsGenerator;

var builder = Host.CreateApplicationBuilder();

var conf = builder.Configuration;

conf.AddWorkerVariables();
builder.Services.AddWorkerInfrastructure(conf);

var role = conf.GetValue<WorkerRole>("WorkerRole");
Console.WriteLine($"Selected role: {role}");

switch (role)
{
    case WorkerRole.Live:
        builder.Services.AddHostedService<LiveAnalyticsProcessor>();
        break;
    case WorkerRole.History:
        builder.Services.AddHostedService<HistoryActivityProcessor>();
        break;
    case WorkerRole.Generator:
        builder.Services.AddHostedService<MockEventPusher>();
        break;
    case WorkerRole.DailyActivityJob:
        builder.Services.AddScoped<DailyActivityAggregator>();
        builder.Services.AddHostedService<DailyActivityJob>();
        break;
    default: 
        throw new ArgumentException("Configuration for worker is not provided");
}

var host = builder.Build();
await host.RunAsync();