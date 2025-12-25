using Portfolio.Api.Data;

public class AnonymousSessionMiddleware
{
    private readonly RequestDelegate _next;

    public AnonymousSessionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Cookies.TryGetValue(Keys.AnonIdCookieName, out var anonId))
        {
            anonId = Guid.NewGuid().ToString();
            context.Response.Cookies.Append(Keys.AnonIdCookieName, anonId, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                SameSite = SameSiteMode.Strict,
                Secure = context.Request.IsHttps
            });
        }

        if (Guid.TryParse(anonId, out var anonGuid))
        {
            context.Items[Keys.AnonSessionGuidKey] = anonGuid;
        }

        await _next(context);
    }
}