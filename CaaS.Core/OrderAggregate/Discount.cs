using CaaS.Core.Base;

namespace CaaS.Core.OrderAggregate; 

public record Discount : IEntityBase {
    public Guid Id { get; init;  } = Guid.NewGuid();
    public string DiscountName { get; init; } = string.Empty;
    public decimal DiscountValue { get; init; } = 0;
    public Guid ShopId { get; init; } = default;
    public Guid ParentId { get; init; } = default;
    public string ConcurrencyToken { get; init; } = string.Empty;
}