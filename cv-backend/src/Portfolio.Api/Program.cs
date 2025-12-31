using System.Text.Json;
using Portfolio.Application;
using Portfolio.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

string[] allowedOrigins;

if (builder.Environment.IsDevelopment())
{
    allowedOrigins = ["http://localhost:3000"];
} 
else
{
    allowedOrigins = ["https://interactive-porfolio.vercel.app"];
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

builder.Services.Configure<GitHubOptions>(builder.Configuration.GetSection("GitHub"));
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
