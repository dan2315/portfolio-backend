using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Portfolio.Api.Data;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireAnonIdentityAttribute : Attribute, IAsyncActionFilter
{

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Items.TryGetValue(Keys.AnonIdCookieName, out var anonObj) || anonObj is not Guid)
        {
            context.Result = new BadRequestObjectResult("Anonymous session not found");
            return;
        }

        await next();
    }
}