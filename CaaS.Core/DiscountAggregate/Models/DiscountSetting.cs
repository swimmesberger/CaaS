using CaaS.Core.Base;

namespace CaaS.Core.DiscountAggregate.Models;

public record DiscountSetting : IEntityBase {
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = string.Empty;
    public DiscountSettingMetadata Rule { get; init; } = DiscountSettingMetadata.Empty;
    public DiscountSettingMetadata Action { get; init; } = DiscountSettingMetadata.Empty;
    public Guid ShopId { get; init; } = Guid.Empty;
    public string ConcurrencyToken { get; init; } = string.Empty;
}