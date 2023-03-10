using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Api.DiscountApi.Models;

public record DiscountSettingForUpdateDto {
    public string Name { get; init; } = string.Empty;
    public DiscountMetadataSettingRaw Rule { get; init; } = DiscountMetadataSettingRaw.Empty;
    public DiscountMetadataSettingRaw Action { get; init; } = DiscountMetadataSettingRaw.Empty;
    public string ConcurrencyToken { get; init; } = string.Empty;
}