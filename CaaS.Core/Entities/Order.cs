using System.Collections.Immutable;
using CaaS.Core.Entities.Base;

namespace CaaS.Core.Entities;

public record Order : IEntityBase {
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid ShopId { get; init; }
    public int OrderNumber { get; init; }
    public Customer Customer { get; init; } = null!;
    
    public IImmutableList<OrderItem> Items { get; init; } = ImmutableArray<OrderItem>.Empty;
    public IImmutableList<Coupon> Coupons { get; init; } = ImmutableArray<Coupon>.Empty;
    public IImmutableList<OrderDiscount> OrderDiscounts { get; init; } = ImmutableArray<OrderDiscount>.Empty;

    public DateTimeOffset OrderDate { get; init; } = DateTimeOffset.UtcNow;
    
    public string ConcurrencyToken { get; init; } = string.Empty;

}