﻿using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using CaaS.Api.Base.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CaaS.Api.Base.Swagger;

public class CaasConventionOperationFilter : IOperationFilter {
    private static readonly IReadOnlyCollection<KeyValuePair<string, string>> ResponseDescriptionMap = new[] {
        new KeyValuePair<string, string>("2\\d{2}", "Success"),

        new KeyValuePair<string, string>("400", "Bad Request"),
        new KeyValuePair<string, string>("404", "Not Found"),
        new KeyValuePair<string, string>("409", "Conflict"),
        new KeyValuePair<string, string>("4\\d{2}", "Client Error"),

        new KeyValuePair<string, string>("5\\d{2}", "Server Error"),
    };

    private const string MediaTypeProblemJson = "application/problem+json";

    private readonly NullabilityInfoContext _nullabilityInfoContext;

    public CaasConventionOperationFilter() {
        _nullabilityInfoContext = new NullabilityInfoContext();
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context) {
        if (ApplyReadFor(operation, context)) return;
        if (ApplyWriteFor(operation, context)) return;

        if (ApplyReadFor<HttpGetAttribute>(operation, context)) return;
        if (ApplyWriteFor<HttpPostAttribute>(operation, context)) return;
        if (ApplyWriteFor<HttpDeleteAttribute>(operation, context)) return;
        if (ApplyWriteFor<HttpPutAttribute>(operation, context)) return;
    }
    
    private bool ApplyReadFor(OpenApiOperation operation, OperationFilterContext context)
        => ApplyReadFor<ReadApiAttribute>(operation, context, static value => value);
    
    private bool ApplyWriteFor(OpenApiOperation operation, OperationFilterContext context)
        => ApplyWriteFor<WriteApiAttribute>(operation, context, static value => value);

    private bool ApplyReadFor<T>(OpenApiOperation operation, OperationFilterContext context, Func<T, ReadApiAttribute>? attributeProvider = null) where T: Attribute {
        if (context.MethodInfo.GetCustomAttribute(typeof(T)) is T attribute) {
            attributeProvider ??= static _ => new ReadApiAttribute();
            ApplyReadApi(operation, context, attributeProvider.Invoke(attribute));
            return true;
        }
        return false;
    }
    
    private bool ApplyWriteFor<T>(OpenApiOperation operation, OperationFilterContext context, Func<T, WriteApiAttribute>? attributeProvider = null) where T: Attribute {
        if (context.MethodInfo.GetCustomAttribute(typeof(T)) is T attribute) {
            attributeProvider ??= static _ => new WriteApiAttribute();
            ApplyWriteApi(operation, context, attributeProvider.Invoke(attribute));
            return true;
        }
        return false;
    }

    private void ApplyReadApi(OpenApiOperation operation, OperationFilterContext context, ReadApiAttribute attribute) {
        var returnType = _nullabilityInfoContext.Create(context.MethodInfo.ReturnParameter);
        if (typeof(Task).IsAssignableFrom(returnType.Type)) {
            var genericParams = returnType.GenericTypeArguments;
            returnType = genericParams.Length >= 1 ? genericParams[0] : null;
        }
        if (returnType == null) return;

        if (returnType.ReadState == NullabilityState.Nullable) {
            AddProblemDetailsByStatusCode(StatusCodes.Status404NotFound, operation, context);
        }
        if (typeof(IEnumerable).IsAssignableFrom(returnType.Type)) {
            var okResponse = operation.Responses.GetValueOrDefault(StatusCodes.Status200OK.ToString());
            if (okResponse == null) return;

            HeaderOperationFilter.ApplyProducesHeader(okResponse, context, new ProducesTotalCountHeaderAttribute());
        }
        ApplyDefaultStatusCodes(operation, context);
    }

    private void ApplyWriteApi(OpenApiOperation operation, OperationFilterContext context, WriteApiAttribute attribute) {
        AddProblemDetailsByStatusCode(StatusCodes.Status404NotFound, operation, context);
        AddProblemDetailsByStatusCode(StatusCodes.Status409Conflict, operation, context);

        ApplyDefaultStatusCodes(operation, context);
    }

    private void ApplyDefaultStatusCodes(OpenApiOperation operation, OperationFilterContext context) {
        AddProblemDetailsByStatusCode(StatusCodes.Status400BadRequest, operation, context);
        
        AddProblemDetailsByStatusCode(StatusCodes.Status500InternalServerError, operation, context);
        AddProblemDetailsByStatusCode(StatusCodes.Status503ServiceUnavailable, operation, context);
    }

    internal static void AddProblemDetailsByStatusCode(int statusCode, OpenApiOperation operation, OperationFilterContext context) {
        var statusCodeString = statusCode.ToString();
        var problemDetailsSchema = context.SchemaGenerator.GenerateSchema(typeof(ProblemDetails), context.SchemaRepository);
        if (operation.Responses.TryGetValue(statusCodeString, out var response)) {
            response.Content[MediaTypeProblemJson] = new OpenApiMediaType { Schema = problemDetailsSchema };
        } else {
            var description = ResponseDescriptionMap
                .FirstOrDefault(entry => Regex.IsMatch(statusCodeString, entry.Key))
                .Value;
            operation.Responses.Add(statusCode.ToString(), new OpenApiResponse {
                Description = description,
                Content = {
                    [MediaTypeProblemJson] = new OpenApiMediaType { Schema = problemDetailsSchema }
                }
            });
        }
    }
}