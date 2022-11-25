using System.Text.Json.Serialization;
using CaaS.Api.Base.Attributes;
using CaaS.Api.Base.Swagger;
using CaaS.Api.DiscountApi.Swagger;
using CaaS.Core;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.Base.Tenant;
using CaaS.Infrastructure.Base;
using CaaS.Infrastructure.Base.Ado.Model;
using FluentValidation;
using FluentValidation.Results;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;

namespace CaaS.Api.Base; 

public static class CaasApiServiceCollectionExtensions {
    public static IServiceCollection AddCaas(this IServiceCollection services, IConfiguration configuration) {
        services.AddAutoMapper(typeof(CaasApiServiceCollectionExtensions));
        services.AddControllers(options => {
            // remove text/plain support
            options.OutputFormatters.RemoveType<StringOutputFormatter>();
        }).AddMvcOptions(options => {
            options.ReturnHttpNotAcceptable = true;
            options.Filters.Add(new NotFoundResultFilterAttribute());
        }).AddJsonOptions(options => {
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.Converters.Add(new OpenApiReferenceJsonConverter());
        });
        services.AddEndpointsApiExplorer();
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantIdAccessor, HttpTenantIdAccessor>();
        services.Configure<RelationalOptions>(configuration.GetSection(RelationalOptions.Key));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<RelationalOptions>>().Value);
        // error handling middleware
        services.AddProblemDetails(ConfigureProblemDetails).AddProblemDetailsConventions();
        services.AddSwaggerGen(options => {
            options.OperationFilter<HeaderOperationFilter>();
            options.OperationFilter<RequireTenantOperationFilter>();
            options.OperationFilter<CaasConventionOperationFilter>();
            options.DocumentFilter<DiscountSettingsOpenApiDocumentFilter>();
        });

        services.AddCaasInfrastructure();
        services.AddCaasCore();
        return services;
    }
    
    private static void ConfigureProblemDetails(ProblemDetailsOptions options) {
        // log exception in development mode
        options.ShouldLogUnhandledException = (context, _, _) 
            => context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment();
        options.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);
        options.MapToStatusCode<HttpRequestException>(StatusCodes.Status503ServiceUnavailable);
        options.MapToStatusCode<CaasUpdateConcurrencyDbException>(StatusCodes.Status409Conflict);
        options.MapToStatusCode<CaasItemNotFoundException>(StatusCodes.Status404NotFound);
        options.MapToStatusCode<CaasNoTenantException>(StatusCodes.Status400BadRequest);
    
        // Custom mapping function for FluentValidation's ValidationException.
        options.Map<ValidationException>((ctx, ex) => HandleValidationErrors(ctx, ex.Errors));
        options.Map<CaasValidationException>((ctx, ex) => HandleValidationErrors(ctx, ex.Errors));

        // Because exceptions are handled polymorphically, this will act as a "catch all" mapping, which is why it's added last.
        // If an exception other than NotImplementedException and HttpRequestException is thrown, this will handle it.
        options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
    }

    private static ValidationProblemDetails HandleValidationErrors(HttpContext ctx, IEnumerable<ValidationFailure> errors) {
        var factory = ctx.RequestServices.GetRequiredService<ProblemDetailsFactory>();
        var errorsDict = errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                x => x.Key,
                x => x.Select(x => x.ErrorMessage).ToArray());

        return factory.CreateValidationProblemDetails(ctx, errorsDict);
    }
}