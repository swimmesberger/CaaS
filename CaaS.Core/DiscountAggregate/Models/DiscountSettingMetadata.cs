namespace CaaS.Core.DiscountAggregate.Models;

public record DiscountSettingMetadata {
    public static readonly DiscountSettingMetadata Empty = new DiscountSettingMetadata();
    
    public Guid Id { get; init; } = Guid.NewGuid();
    public DiscountParameters Parameters { get; init; } = DiscountParameters.Empty;
}