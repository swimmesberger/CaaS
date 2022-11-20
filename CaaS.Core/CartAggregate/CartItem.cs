using System.Collections.Immutable;
using CaaS.Core.Base;
using CaaS.Core.OrderAggregate;
using CaaS.Core.ProductAggregate;

namespace CaaS.Core.CartAggregate; 

public record CartItem : IEntityBase {
    public Guid Id { get; init; } = Guid.NewGuid();
    
    public Product Product { get; init; } = null!;
    public Guid ShopId { get; init; } = default;
    public Guid CartId { get; init; } = default;
    public int Amount { get; init; } = 0;
    public IImmutableList<ItemDiscount> CartItemDiscounts { get; init; } = ImmutableArray<ItemDiscount>.Empty;
    public string ConcurrencyToken { get; init; } = string.Empty;
}