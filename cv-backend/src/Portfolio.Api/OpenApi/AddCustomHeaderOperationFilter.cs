using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

public class AddCustomHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
        {
            operation.Parameters = [];
        }

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Admin-Key",
            In = ParameterLocation.Header,
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String
            },
            Description = "Custom header for all requests"
        });
    }
}
