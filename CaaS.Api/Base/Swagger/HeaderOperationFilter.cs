﻿using System.Reflection;
using CaaS.Api.Base.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CaaS.Api.Base.Swagger;

public class HeaderOperationFilter : IOperationFilter {
    public void Apply(OpenApiOperation operation, OperationFilterContext context) {
        ApplyProducesHeader(operation, context);
        ApplyConsumesHeader(operation, context);
    }

    private void ApplyProducesHeader(OpenApiOperation operation, OperationFilterContext context) {
        if (context.MethodInfo.GetCustomAttribute(typeof(ProducesHeaderAttribute)) is not ProducesHeaderAttribute attribute) return;
        foreach (var openApiResponse in operation.Responses.Values) {
            ApplyProducesHeader(openApiResponse, context, attribute);
        }
    }
    
    private void ApplyConsumesHeader(OpenApiOperation operation, OperationFilterContext context) {
        if (context.MethodInfo.GetCustomAttribute(typeof(ConsumesHeaderAttribute)) is not ConsumesHeaderAttribute attribute) return;
        ApplyConsumesHeader(operation, context, attribute);
    }
    
    internal static void ApplyProducesHeader(OpenApiResponse openApiResponse, OperationFilterContext context, ProducesHeaderAttribute attribute) {
        openApiResponse.Headers[attribute.Name] = new OpenApiHeader() {
            Description = attribute.Description,
            Schema = attribute.Type == null ? null : context.SchemaGenerator
                .GenerateSchema(attribute.Type, context.SchemaRepository)
        };
    }
    
    internal static void ApplyConsumesHeader(OpenApiOperation operation, OperationFilterContext context, ConsumesHeaderAttribute attribute) {
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