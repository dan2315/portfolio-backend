var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient<LeetCodeService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();
app.UseCors("AllowAll");

app.MapGet("/leetcode/profile", async (LeetCodeService leetcodeService) =>
{
    var stats = await leetcodeService.GetUserStatsAsync("dan2315");
    if (stats == null)
    {
        return Results.NotFound();
    }
    
    return Results.Ok(stats);
});

app.MapGet("/leetcode/languages", async (LeetCodeService leetcodeService) =>
{
    var langData = await leetcodeService.GetLanguagesData("dan2315");
    Console.WriteLine("Lang Data" + langData);
    if (langData == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(langData);
});

app.MapGet("/leetcode/submissions", async (LeetCodeService leetcodeService) =>
{
    var submissions = await leetcodeService.GetRecentSubmissionsAsync("dan2315");
    if (submissions == null)
    {
        return Results.NotFound();
    }
    
    return Results.Ok(submissions);
});

app.MapGet("/leetcode/activity", async (LeetCodeService leetcodeService) =>
{
    var activity = await leetcodeService.GetUserActivityCalendarAsync("dan2315");
    if (activity == null)
    {
        return Results.NotFound();
    }
    
    return Results.Ok(activity);
});

app.Run();

