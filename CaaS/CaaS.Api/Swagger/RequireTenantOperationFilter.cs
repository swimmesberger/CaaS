﻿using System.Reflection;
using CaaS.Api.Middleware;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CaaS.Api.Swagger; 

public class RequireTenantOperationFilter : IOperationFilter {
    public void Apply(OpenApiOperation operation, OperationFilterContext context) {
        if (context.MethodInfo.DeclaringType?.GetCustomAttribute(typeof(RequireTenantAttribute)) is not RequireTenantAttribute) return;
        operation.Parameters ??= new List<OpenApiParameter>();
        operation.Parameters.Add(new OpenApiParameter {
                Name = HeaderConstants.TenantId,
                In = ParameterLocation.Header,
                Description = HeaderConstants.TenantIdDescription,
                Required = true,
                Schema = context.SchemaGenerator.GenerateSchema(typeof(Guid), context.SchemaRepository)
        });
    }
}