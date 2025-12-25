using Portfolio.Application;
using Portfolio.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.Configure<GitHubOptions>(builder.Configuration.GetSection("GitHub"));
builder.Services.AddInfrastruture(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
   options.SwaggerDoc("v1", new()
   {
       Title = "Portfolio API",
       Version = "v1",
       Description = "idk"
   });
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
