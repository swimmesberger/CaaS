using System.Collections.Immutable;
using CaaS.Core.Base;

namespace CaaS.Core.Order.Entities;

public record OrderItem : IEntityBase {
    public Guid Id { get; init;  } = Guid.NewGuid();
    
    public Product.Entities.Product Product { get; init; } = null!;
    public Guid ShopId { get; init; }
    public Guid OrderId { get; init; }
    public int Amount { get; init; }
    
    public IImmutableList<OrderItemDiscount> OrderItemDiscounts { get; init; } = ImmutableArray<OrderItemDiscount>.Empty;
    public decimal PricePerPiece { get; init; }
    public string ConcurrencyToken { get; init; } = string.Empty;
}