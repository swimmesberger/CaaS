using System.Collections;
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
        new KeyValuePair<string, string>("409", "Conflict")
    };
    
     private readonly NullabilityInfoContext _nullabilityInfoContext;

     public CaasConventionOperationFilter() {
         _nullabilityInfoContext = new NullabilityInfoContext();
     }

     public void Apply(OpenApiOperation operation, OperationFilterContext context) {
        if (context.MethodInfo.GetCustomAttribute(typeof(WriteApiAttribute)) is WriteApiAttribute writeApiAttribute) {
            ApplyWriteApi(operation, context, writeApiAttribute);
            return;
        }
        if (context.MethodInfo.GetCustomAttribute(typeof(ReadApiAttribute)) is ReadApiAttribute readApiAttribute) {
            ApplyReadApi(operation, context, readApiAttribute);
            return;
        }
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
     }

    private void ApplyWriteApi(OpenApiOperation operation, OperationFilterContext context, WriteApiAttribute attribute) {
        AddProblemDetailsByStatusCode(StatusCodes.Status404NotFound, operation, context);
        AddProblemDetailsByStatusCode(StatusCodes.Status409Conflict, operation, context);
    }

    internal static void AddProblemDetailsByStatusCode(int statusCode, OpenApiOperation operation, OperationFilterContext context) {
        var statusCodeString = statusCode.ToString();
        var problemDetailsSchema = context.SchemaGenerator.GenerateSchema(typeof(ProblemDetails), context.SchemaRepository);
        if (operation.Responses.TryGetValue(statusCodeString, out var response)) {
            response.Content["application/problem+json"] = new OpenApiMediaType { Schema = problemDetailsSchema };
        } else {
            var description = ResponseDescriptionMap
                .FirstOrDefault(entry => Regex.IsMatch(statusCodeString, entry.Key))
                .Value;
            operation.Responses.Add(statusCode.ToString(), new OpenApiResponse {
                Description = description,
                Content = {
                    ["application/problem+json"] = new OpenApiMediaType { Schema = problemDetailsSchema }
                }
            });
        }
    }
}