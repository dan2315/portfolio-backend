using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Portfolio.Infrastructure;
using Workers.Analytics;
using Workers.Analytics.Configuration;

var builder = Host.CreateApplicationBuilder();

var conf = builder.Configuration;

conf.AddWorkerVariables();
builder.Services.AddInfrastructure(conf);
builder.Services.AddHostedService<AnalyticsProcessor>();

var host = builder.Build();
await host.RunAsync();