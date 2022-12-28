using System.Text.Json.Serialization;
using CaaS.Api.BackgroundServices;
using CaaS.Api.Base.AppKey;
using CaaS.Api.Base.Attributes;
using CaaS.Api.Base.Swagger;
using CaaS.Api.DiscountApi.Swagger;
using CaaS.Core;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.Base.Pagination;
using CaaS.Core.Base.Tenant;
using CaaS.Core.Base.Url;
using CaaS.Core.Base.Validation;
using CaaS.Infrastructure.Base;
using CaaS.Infrastructure.Base.Ado.Model;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace CaaS.Api.Base; 

public static class CaasApiServiceCollectionExtensions {
    public const string CorsAllowSpecificOrigins = "_corsAllowSpecificOrigins";
    
    public static IServiceCollection AddCaas(this IServiceCollection services, IConfiguration configuration) {
        services.AddAutoMapper(typeof(CaasApiServiceCollectionExtensions));
        services.AddControllers(options => {
            options.ReturnHttpNotAcceptable = true;
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
        // OpenId Azure AD Auth
        // builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //         .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));
        services.AddAuthentication().AddAppKey();
        services.AddAuthorization(options => {
            options.AddPolicy(AppKeyAuthenticationDefaults.AuthorizationPolicy, policy => {
                policy.AuthenticationSchemes.Add(AppKeyAuthenticationDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
            });
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
            options.OperationFilter<AuthorizeOperationFilter>();
            options.OperationFilter<CaasConventionOperationFilter>();
            options.DocumentFilter<DiscountSettingsOpenApiDocumentFilter>();
            options.AddSecurityDefinition(AppKeyAuthenticationDefaults.AuthenticationScheme, new OpenApiSecurityScheme {
                Description = @$"AppKey Authorization header using the AppKey scheme. <br /><br />
                      Enter '{HeaderConstants.AppKey}' [space] and then your token in the text input below.
                      <br /><br />Example: '{HeaderConstants.AppKey} 362a9325-ffb8-432b-bfd3-91c191fd5d69'",
                Name = HeaderConstants.AppKey,
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = AppKeyAuthenticationDefaults.AuthenticationScheme
            });
            options.MapType<SkipTokenValue>(() => new OpenApiSchema { Type = "string" });
        });
        services.AddCors(options => {
            options.AddPolicy(name: CorsAllowSpecificOrigins,
                builder => {
                    var origins = new List<string> {
                        "https://localhost:4200",
                        "http://localhost:4200"
                    };
                    builder.WithOrigins(origins.ToArray()).AllowAnyHeader().AllowAnyMethod();
                }
            );
        });
        services.AddHostedService<CartCleanupService>();
        services.AddScoped<IValidator, WebValidator>();
        services.AddScoped<ILinkGenerator, WebBlobLinkGenerator>();

        services.AddCaasInfrastructure();
        services.AddCaasCore();
        return services;
    }

    // ReSharper disable once UnusedMethodReturnValue.Local
    private static AuthenticationBuilder AddAppKey(this AuthenticationBuilder builder) {
        builder.Services.AddOptions<AppKeyAuthenticationOptions>(AppKeyAuthenticationDefaults.AuthenticationScheme);
        return builder.AddScheme<AppKeyAuthenticationOptions, AppKeyAuthenticationHandler>(AppKeyAuthenticationDefaults.AuthenticationScheme, null, null!);
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
    
        // Custom mapping function for ValidationException.
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
                x => x.Select(failure => failure.ErrorMessage).ToArray());

        return factory.CreateValidationProblemDetails(ctx, errorsDict);
    }
}