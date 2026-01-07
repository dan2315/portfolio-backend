using System.Text.Json;
using Portfolio.Application;
using Portfolio.Application.Options;
using Portfolio.Application.StaticContent;
using Portfolio.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

string[] allowedOrigins;
string? dbConnectionString;

if (builder.Environment.IsDevelopment())
{
    allowedOrigins = ["http://localhost:3000"];
    dbConnectionString = builder.Configuration.GetConnectionString("Postgres");
} 
else
{
    allowedOrigins = [
        "https://interactive-porfolio.vercel.app",
        "https://portfolio.danielthedod.dev/",
        "https://www.portfolio.danielthedod.dev/",
        ];
    dbConnectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION");
    Console.WriteLine("Connection string " + dbConnectionString);
    if (string.IsNullOrEmpty(dbConnectionString))
    {
        throw new InvalidOperationException("POSTGRES_CONNECTION environment variable is not set.");
    }
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables();

builder.Services.Configure<GitHubOptions>(builder.Configuration.GetSection("GitHub"));
builder.Services.Configure<SendGridOptions>(builder.Configuration.GetSection("SendGrid"));
builder.Configuration["ConnectionStrings:Postgres"] = dbConnectionString;

builder.Services.AddSingleton(sp => 
    new StaticContentPath(Path.Combine(sp.GetRequiredService<IWebHostEnvironment>().ContentRootPath, "StaticContent")));
builder.Services.AddInfrastructure(builder.Configuration);
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
