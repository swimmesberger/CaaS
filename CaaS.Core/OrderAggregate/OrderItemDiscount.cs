using CaaS.Core.Base;

namespace CaaS.Core.OrderAggregate; 

public class OrderItemDiscount : IEntityBase {
    public Guid Id { get; init; } = Guid.NewGuid();

    public Guid ShopId { get; init; } = default;
    public Guid OrderItemId { get; init; } = default;
    public string DiscountName { get; init; } = string.Empty;
    public decimal DiscountValue { get; init; } = 0;
    public string ConcurrencyToken { get; init; } = string.Empty;
}