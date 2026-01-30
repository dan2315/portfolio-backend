using System.Text.Json;
using Portfolio.Api.Configuration;
using Portfolio.Api.Controllers;
using Portfolio.Application;
using Portfolio.Application.Options;
using Portfolio.Application.StaticContent;
using Portfolio.Infrastructure.Analytics.InterappTransport;
using Portfolio.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment;
var conf = builder.Configuration;

builder.ConfigureKestrelForDebug();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(env.GetAllowedOrigins())
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

conf.AddApplicationVariables(env);

builder.Services.Configure<GitHubOptions>(builder.Configuration.GetSection("GitHub"));
builder.Services.Configure<SendGridOptions>(builder.Configuration.GetSection("SendGrid"));

builder.Services.AddSingleton(sp => 
    new StaticContentPath(Path.Combine(sp.GetRequiredService<IWebHostEnvironment>().ContentRootPath, "StaticContent")));
builder.Services.AddWebInfrastructure(conf);
builder.Services.AddApplication();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
   options.SwaggerDoc("v1", new()
   {
       Title = "Portfolio API",
       Version = "v1",
       Description = "idk"
   });

    options.OperationFilter<AddCustomHeaderOperationFilter>();
});

var app = builder.Build();
app.UseCors("Frontend");
app.UseMiddleware<AnonymousSessionMiddleware>();
app.UseMiddleware<ActivityTrackingMiddleware>();
app.MapControllers();
AnalyticsController.MapExtraRoutes(app);
app.MapGrpcService<SessionDeltaGrpcService>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
