using System.Collections.Immutable;
using CaaS.Core.Discount;
using CaaS.Core.Entities.Base;

namespace CaaS.Core.Entities;

public record OrderItem : IEntityBase {
    public Guid Id { get; init;  } = Guid.NewGuid();
    
    public Product Product { get; init; } = null!;
    public Guid ShopId { get; init; } = default;
    public Guid OrderId { get; init; } = default;
    public int Amount { get; init; }
    
    public IImmutableList<OrderItemDiscount> OrderItemDiscounts { get; init; } = ImmutableList<OrderItemDiscount>.Empty;
    public decimal PricePerPiece { get; init; }
    public string ConcurrencyToken { get; init; } = string.Empty;
}