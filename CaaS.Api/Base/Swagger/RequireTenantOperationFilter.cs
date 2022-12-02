using CaaS.Api.Base.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CaaS.Api.Base.Swagger; 

public class RequireTenantOperationFilter : IOperationFilter {
    public void Apply(OpenApiOperation operation, OperationFilterContext context) {
        var attribute = context.MethodInfo.GetAttribute<RequireTenantAttribute>();
        if (attribute == null) return;
        
        operation.Parameters ??= new List<OpenApiParameter>();
        operation.Parameters.Add(new OpenApiParameter {
                Name = HeaderConstants.TenantId,
                In = ParameterLocation.Header,
                Description = HeaderConstants.TenantIdDescription,
                Required = true,
                Schema = context.SchemaGenerator.GenerateSchema(typeof(Guid), context.SchemaRepository)
        });
        CaasConventionOperationFilter.AddProblemDetailsByStatusCode(StatusCodes.Status400BadRequest, operation, context);
    }
}