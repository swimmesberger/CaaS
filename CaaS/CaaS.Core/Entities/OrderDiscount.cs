using CaaS.Core.Entities.Base;

namespace CaaS.Core.Entities; 

public class OrderDiscount : IEntityBase {
    public Guid Id { get; init;  } = Guid.NewGuid();
    public string DiscountName { get; init; } = string.Empty;
    public decimal DiscountValue { get; init; } = 0;
    public Guid ShopId { get; init; } = default;
    public Guid OrderId { get; init; } = default;
    public string ConcurrencyToken { get; init; } = string.Empty;
}