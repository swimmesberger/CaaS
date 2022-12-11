namespace CaaS.Core.DiscountAggregate.Models;

public record DiscountSettingRaw {
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DiscountMetadataSettingRaw Rule { get; init; } = DiscountMetadataSettingRaw.Empty;
    public DiscountMetadataSettingRaw Action { get; init; } = DiscountMetadataSettingRaw.Empty;
    public Guid ShopId { get; init; }
    public string ConcurrencyToken { get; init; } = string.Empty;
}