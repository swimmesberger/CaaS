using CaaS.Api.Base.Attributes;
using CaaS.Api.DiscountApi.Models;
using CaaS.Core.DiscountAggregate.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CaaS.Api.DiscountApi; 

[ApiController]
[Route("[controller]")]
[CaasApiConvention]
public class DiscountController : ControllerBase {
    private readonly IDiscountService _discountService;
    private readonly SchemaGeneratorOptions _schemaGeneratorOptions;

    public DiscountController(IDiscountService discountService, SchemaGeneratorOptions schemaGeneratorOptions) {
        _discountService = discountService;
        _schemaGeneratorOptions = schemaGeneratorOptions;
    }

    /// <summary>
    /// Returns possible actions and rules (not configured rules)
    /// </summary>
    /// <returns></returns>
    [HttpGet("components")]
    public DiscountComponentsDto GetDiscountComponents() {
        return ToViewModel(_discountService.GetDiscountMetadata());
    }
    
    private DiscountComponentsDto ToViewModel(IEnumerable<DiscountComponentMetadata> discountComponents) {
        var componentsList = discountComponents.ToList();
        var rules = componentsList.Where(c => c.ComponentType == DiscountComponentType.Rule)
            .Select(ToViewModel).ToList();
        var actions = componentsList.Where(c => c.ComponentType == DiscountComponentType.Action)
            .Select(ToViewModel).ToList();
        return new DiscountComponentsDto(rules, actions);
    }

    private DiscountComponentDto ToViewModel(DiscountComponentMetadata componentMetadata) {
        return new DiscountComponentDto(componentMetadata.Id, 
            new DiscountComponentParametersDto(CreateSchemaReference(componentMetadata.SettingsType)), 
            componentMetadata.ComponentType);
    }

    private OpenApiReference CreateSchemaReference(Type type) {
        var settingsSchema = _schemaGeneratorOptions.SchemaIdSelector.Invoke(type);
        return new OpenApiReference() { Id = settingsSchema, Type = ReferenceType.Schema };
    }
}