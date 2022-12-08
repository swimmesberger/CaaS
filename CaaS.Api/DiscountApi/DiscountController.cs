using System.Text.Json;
using AutoMapper;
using CaaS.Api.Base.Attributes;
using CaaS.Api.DiscountApi.Models;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.DiscountAggregate.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CaaS.Api.DiscountApi; 

[ApiController]
[Route("[controller]")]
[CaasApiConvention]
[RequireTenant]
public class DiscountController : ControllerBase {
    private readonly IDiscountService _discountService;
    private readonly SchemaGeneratorOptions _schemaGeneratorOptions;
    private readonly IDiscountComponentProvider _discountComponentProvider;
    private readonly JsonOptions _jsonOptions;

    public DiscountController(IDiscountService discountService, SchemaGeneratorOptions schemaGeneratorOptions,
        IDiscountComponentProvider discountComponentProvider, IOptions<JsonOptions> jsonOptions) {
        _discountService = discountService;
        _schemaGeneratorOptions = schemaGeneratorOptions;
        _discountComponentProvider = discountComponentProvider;
        _jsonOptions = jsonOptions.Value;
    }

    /// <summary>
    /// Returns possible actions and rules (not configured rules)
    /// </summary>
    /// <returns></returns>
    [HttpGet("components")]
    public DiscountComponentsDto GetDiscountComponents() {
        return ToViewModel(_discountService.GetDiscountMetadata());
    }

    [HttpPost()]
    public async Task<CreatedAtActionResult> AddDiscountSetting(DiscountSettingForCreationDto forCreationDto,
        CancellationToken cancellationToken = default) {
        
        var ruleMetadata = _discountComponentProvider.GetDiscountMetadataById(forCreationDto.Rule.Id);
        if (ruleMetadata == null) {
            throw new CaasValidationException($"ruleId '{forCreationDto.Action.Id}' invalid");
        }
        
        var actionMetadata = _discountComponentProvider.GetDiscountMetadataById(forCreationDto.Action.Id);
        if (actionMetadata == null) {
            throw new CaasValidationException($"actionId '{forCreationDto.Action.Id}' invalid");
        }

        var rule = (DiscountParameters) JsonSerializer.Deserialize(forCreationDto.Rule.Parameters, ruleMetadata.SettingsType, _jsonOptions.JsonSerializerOptions)!;
        var action = (DiscountParameters) JsonSerializer.Deserialize(forCreationDto.Action.Parameters, actionMetadata.SettingsType, _jsonOptions.JsonSerializerOptions)!;

        if(!TryValidateModel(rule)){
            throw new CaasValidationException("rule not valid");
        }

        if (!TryValidateModel(action)) {
            throw new CaasValidationException("action not valid");
        }

        var discountSetting = new DiscountSetting {
            Id = forCreationDto.Id,
            Name = forCreationDto.Name,
            Rule = new DiscountSettingMetadata {
                Id = forCreationDto.Rule.Id,
                Parameters = rule
            },
            Action = new DiscountSettingMetadata {
                Id = forCreationDto.Action.Id,
                Parameters = action
            }
        };

        var result = await _discountService.AddDiscountSettingAsync(discountSetting, cancellationToken);

        return CreatedAtAction(
            actionName: nameof(AddDiscountSetting),
            routeValues: new { discountSettingId = result.Id },
            value: new DiscountSettingDto(result.Id, result.Name, forCreationDto.Rule, forCreationDto.Action, result.ShopId, result.ConcurrencyToken));
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