using CaaS.Api.Base.AppKey;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CaaS.Api.Base.Swagger; 

public sealed class AuthorizeOperationFilter : IOperationFilter {
    public void Apply(OpenApiOperation operation, OperationFilterContext context) {
        var authorizeAttribute = context.MethodInfo.GetAttribute<AuthorizeAttribute>();
        if (authorizeAttribute != null) {
            CaasConventionOperationFilter.AddProblemDetailsByStatusCode(StatusCodes.Status401Unauthorized, operation, context);
            AddAppKeySecurity(operation);
        }
    }
    
    private void AddAppKeySecurity(OpenApiOperation operation) {
        operation.Security = new List<OpenApiSecurityRequirement>() {
            new() {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = AppKeyAuthenticationDefaults.AuthenticationScheme
                        },
                        Scheme = AppKeyAuthenticationDefaults.AuthenticationScheme,
                        Name = AppKeyAuthenticationDefaults.AuthenticationScheme,
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            }
        };
    }
}