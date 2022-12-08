using System.Reflection.Metadata.Ecma335;
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

    [HttpGet()]
    public async Task<IEnumerable<DiscountSettingDto>> GetAll(CancellationToken cancellationToken = default) {
        var result = await _discountService.GetAllDiscountSettingsAsync(cancellationToken);
        //TODO: mapping of parameters
        return new List<DiscountSettingDto>();
    }
    
    [HttpPost()]
    public async Task<CreatedAtActionResult> AddDiscountSetting(DiscountSettingForCreationOrUpdateDto forCreationOrUpdateDto,
        CancellationToken cancellationToken = default) {
        
        ValidateDiscountSetting(forCreationOrUpdateDto, out var rule, out var action);

        var discountSetting = new DiscountSetting {
            Id = forCreationOrUpdateDto.Id,
            Name = forCreationOrUpdateDto.Name,
            Rule = new DiscountSettingMetadata {
                Id = forCreationOrUpdateDto.Rule.Id,
                Parameters = rule
            },
            Action = new DiscountSettingMetadata {
                Id = forCreationOrUpdateDto.Action.Id,
                Parameters = action
            }
        };

        var result = await _discountService.AddDiscountSettingAsync(discountSetting, cancellationToken);

        return CreatedAtAction(
            actionName: nameof(AddDiscountSetting),
            routeValues: new { discountSettingId = result.Id },
            value: new DiscountSettingDto(result.Id, result.Name, forCreationOrUpdateDto.Rule, forCreationOrUpdateDto.Action, result.ShopId, result.ConcurrencyToken));
    }

    [HttpPut("{discountSettingId:guid}")]
    public async Task<DiscountSettingDto> UpdateDiscountSetting(Guid discountSettingId, DiscountSettingForCreationOrUpdateDto forCreationOrUpdateDto,
        CancellationToken cancellationToken = default) {
        
        ValidateDiscountSetting(forCreationOrUpdateDto, out var rule, out var action);
        
        var discountSetting = new DiscountSetting {
            Id = forCreationOrUpdateDto.Id,
            Name = forCreationOrUpdateDto.Name,
            Rule = new DiscountSettingMetadata {
                Id = forCreationOrUpdateDto.Rule.Id,
                Parameters = rule
            },
            Action = new DiscountSettingMetadata {
                Id = forCreationOrUpdateDto.Action.Id,
                Parameters = action
            }
        };
        
        var result = await _discountService.UpdateDiscountSettingAsync(discountSettingId, discountSetting, cancellationToken);
        return new DiscountSettingDto(result.Id, result.Name, forCreationOrUpdateDto.Rule, forCreationOrUpdateDto.Action, result.ShopId, result.ConcurrencyToken);
    }

    [HttpDelete("{discountSettingId:guid}")]
    public async Task<ActionResult> DeleteDiscountSetting(Guid discountSettingId, CancellationToken cancellationToken = default) {
        await _discountService.DeleteDiscountSettingAsync(discountSettingId, cancellationToken);
        return NoContent();
    }

    private void ValidateDiscountSetting(DiscountSettingForCreationOrUpdateDto forCreationOrUpdateDto, out DiscountParameters rule, out DiscountParameters action) {
        var ruleMetadata = _discountComponentProvider.GetDiscountMetadataById(forCreationOrUpdateDto.Rule.Id);
        if (ruleMetadata == null) {
            throw new CaasValidationException($"ruleId '{forCreationOrUpdateDto.Action.Id}' invalid");
        }
        
        var actionMetadata = _discountComponentProvider.GetDiscountMetadataById(forCreationOrUpdateDto.Action.Id);
        if (actionMetadata == null) {
            throw new CaasValidationException($"actionId '{forCreationOrUpdateDto.Action.Id}' invalid");
        }

        rule = (DiscountParameters) JsonSerializer.Deserialize(forCreationOrUpdateDto.Rule.Parameters, ruleMetadata.SettingsType, _jsonOptions.JsonSerializerOptions)!;
        action = (DiscountParameters) JsonSerializer.Deserialize(forCreationOrUpdateDto.Action.Parameters, actionMetadata.SettingsType, _jsonOptions.JsonSerializerOptions)!;

        if(!TryValidateModel(rule)){
            throw new CaasValidationException("rule not valid");
        }

        if (!TryValidateModel(action)) {
            throw new CaasValidationException("action not valid");
        }
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