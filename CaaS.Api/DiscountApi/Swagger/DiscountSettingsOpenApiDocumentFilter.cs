using CaaS.Core.DiscountAggregate.Base;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CaaS.Api.DiscountApi.Swagger; 

public class DiscountSettingsOpenApiDocumentFilter : IDocumentFilter {
    private readonly IDiscountComponentFactory _discountComponentFactory;

    public DiscountSettingsOpenApiDocumentFilter(IDiscountComponentFactory discountService) {
        _discountComponentFactory = discountService;
    }
    
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context) {
        var refSchema = context.SchemaGenerator.GenerateSchema(typeof(OpenApiReference), context.SchemaRepository);
        context.SchemaRepository.Schemas[refSchema.Reference.Id] = new OpenApiSchema() {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>() { [OpenApiConstants.DollarRef] = new OpenApiSchema { Type = "string" } },
            AdditionalPropertiesAllowed = false
        };
        foreach (var discountComponentMetadata in _discountComponentFactory.GetDiscountMetadata()) {
            context.SchemaGenerator.GenerateSchema(discountComponentMetadata.SettingsType, context.SchemaRepository);
        }
    }
}