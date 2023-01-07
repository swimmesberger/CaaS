using System.Text.Json;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Api.DiscountApi.Models;

public record DiscountSettingForCreationDto {
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DiscountMetadataSettingRawForCreationDto Rule { get; init; } = DiscountMetadataSettingRawForCreationDto.Empty;
    public DiscountMetadataSettingRawForCreationDto Action { get; init; } = DiscountMetadataSettingRawForCreationDto.Empty;
}

public record DiscountMetadataSettingRawForCreationDto {
    public static readonly DiscountMetadataSettingRawForCreationDto Empty = new DiscountMetadataSettingRawForCreationDto();
    
    public Guid? Id { get; init; }
    public JsonElement Parameters { get; init; }
}