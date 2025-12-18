public class AnonymousSessionMiddleware
{
    private readonly RequestDelegate _next;
    private const string CookieName = "anonId";

    public AnonymousSessionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Cookies.TryGetValue(CookieName, out var anonId))
        {
            anonId = Guid.NewGuid().ToString();
            context.Response.Cookies.Append(CookieName, anonId, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                SameSite = SameSiteMode.Strict,
                Secure = context.Request.IsHttps
            });
        }

        context.Items[CookieName] = anonId;

        await _next(context);
    }
}
