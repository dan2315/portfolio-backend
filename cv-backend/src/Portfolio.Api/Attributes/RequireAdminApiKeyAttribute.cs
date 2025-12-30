using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireAdminApiKeyAttribute : Attribute, IAuthorizationFilter
{
    private const string HeaderName = "X-Admin-Key";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var configuration = context.HttpContext.RequestServices
            .GetRequiredService<IConfiguration>();

        var expectedKey = configuration["Admin:ApiKey"];
        var providedKey = context.HttpContext.Request.Headers[HeaderName].FirstOrDefault();

        if (string.IsNullOrEmpty(expectedKey) ||
            string.IsNullOrEmpty(providedKey) ||
            !TimingSafeEquals(expectedKey, providedKey))
        {
            context.Result = new UnauthorizedResult();
        }
    }

    private static bool TimingSafeEquals(string a, string b)
    {
        if (a.Length != b.Length) return false;
        var result = 0;
        for (int i = 0; i < a.Length; i++)
            result |= a[i] ^ b[i];
        return result == 0;
    }
}
