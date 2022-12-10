using AutoMapper;
using CaaS.Api.Base.Attributes;
using CaaS.Api.DiscountApi.Models;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.DiscountAggregate.Models;
using Microsoft.AspNetCore.Mvc;
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
    private readonly IMapper _mapper;

    public DiscountController(IDiscountService discountService, SchemaGeneratorOptions schemaGeneratorOptions, IMapper mapper) {
        _discountService = discountService;
        _schemaGeneratorOptions = schemaGeneratorOptions;
        _mapper = mapper;
    }

    /// <summary>
    /// Returns possible actions and rules (not configured rules)
    /// </summary>
    /// <returns></returns>
    [HttpGet("components")]
    public DiscountComponentsDto GetDiscountComponents() {
        return ToViewModel(_discountService.GetDiscountMetadata());
    }

    [HttpGet]
    public async Task<IEnumerable<DiscountSettingRaw>> GetAll(CancellationToken cancellationToken = default) {
        return await _discountService.GetAllDiscountSettingsAsync(cancellationToken);
    }
    
    [HttpPost]
    public async Task<CreatedAtActionResult> AddDiscountSetting(DiscountSettingForCreationOrUpdateDto forCreationOrUpdateDto, CancellationToken cancellationToken = default) {
        var discountSettingRaw = _mapper.Map<DiscountSettingRaw>(forCreationOrUpdateDto);
        var result = await _discountService.AddDiscountSettingAsync(discountSettingRaw, cancellationToken);
        return CreatedAtAction(
            actionName: nameof(AddDiscountSetting),
            routeValues: new { discountSettingId = result.Id },
            value: result);
    }

    [HttpPut("{discountSettingId:guid}")]
    public async Task<DiscountSettingRaw> UpdateDiscountSetting(Guid discountSettingId, DiscountSettingForCreationOrUpdateDto forCreationOrUpdateDto,
        CancellationToken cancellationToken = default) {
        forCreationOrUpdateDto = forCreationOrUpdateDto with { Id = discountSettingId };
        var discountSettingRaw = _mapper.Map<DiscountSettingRaw>(forCreationOrUpdateDto);
        return await _discountService.UpdateDiscountSettingAsync(discountSettingRaw, cancellationToken);
    }

    [HttpDelete("{discountSettingId:guid}")]
    public async Task<ActionResult> DeleteDiscountSetting(Guid discountSettingId, CancellationToken cancellationToken = default) {
        await _discountService.DeleteDiscountSettingAsync(discountSettingId, cancellationToken);
        return NoContent();
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