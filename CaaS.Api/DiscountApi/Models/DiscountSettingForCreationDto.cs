using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Api.DiscountApi.Models;

public record DiscountSettingForCreationDto {
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DiscountMetadataSettingRaw Rule { get; init; } = DiscountMetadataSettingRaw.Empty;
    public DiscountMetadataSettingRaw Action { get; init; } = DiscountMetadataSettingRaw.Empty;
}