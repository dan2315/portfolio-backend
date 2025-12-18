using Portfolio.Application;
using Portfolio.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient<LeetCodeService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});
builder.Services.AddInfrastruture();
builder.Services.AddApplication();
builder.Services.AddControllers();
var app = builder.Build();
app.UseMiddleware<AnonymousSessionMiddleware>();
app.MapControllers();
app.UseCors("AllowAll");
app.Run();
