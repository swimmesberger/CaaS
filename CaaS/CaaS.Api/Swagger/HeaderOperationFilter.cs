using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CaaS.Api.Swagger;

public class HeaderOperationFilter : IOperationFilter {
    public void Apply(OpenApiOperation operation, OperationFilterContext context) {
        ApplyProducesHeader(operation, context);
        ApplyConsumesHeader(operation, context);
    }

    private void ApplyProducesHeader(OpenApiOperation operation, OperationFilterContext context) {
        if (context.MethodInfo.GetCustomAttribute(typeof(ProducesHeaderAttribute)) is not ProducesHeaderAttribute attribute) return;

        foreach (var openApiResponse in operation.Responses.Values) {
            openApiResponse.Headers[attribute.Name] = new OpenApiHeader() {
                Description = attribute.Description,
                Schema = attribute.Type == null ? null : context.SchemaGenerator
                        .GenerateSchema(attribute.Type, context.SchemaRepository)
            };
        }
    }
    
    private void ApplyConsumesHeader(OpenApiOperation operation, OperationFilterContext context) {
        if (context.MethodInfo.GetCustomAttribute(typeof(ConsumesHeaderAttribute)) is not ConsumesHeaderAttribute attribute) return;
        
        var existingParam = operation.Parameters.FirstOrDefault(p =>
                p.In == ParameterLocation.Header && p.Name == attribute.Name);
        // remove description from [FromHeader] argument attribute
        if (existingParam != null) {
            operation.Parameters.Remove(existingParam);
        }
        operation.Parameters ??= new List<OpenApiParameter>();
        operation.Parameters.Add(new OpenApiParameter {
                Name = attribute.Name,
                In = ParameterLocation.Header,
                Description = attribute.Description,
                Required = attribute.IsRequired,
                Schema = attribute.Type == null ? null : context.SchemaGenerator
                        .GenerateSchema(attribute.Type, context.SchemaRepository)
        });
    }
}