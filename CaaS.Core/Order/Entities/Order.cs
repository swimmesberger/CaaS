using System.Collections.Immutable;
using CaaS.Core.Base;

namespace CaaS.Core.Order.Entities;

public record Order : IEntityBase {
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid ShopId { get; init; }
    public int OrderNumber { get; init; }
    public Customer.Entities.Customer Customer { get; init; } = null!;
    
    public IImmutableList<OrderItem> Items { get; init; } = ImmutableArray<OrderItem>.Empty;
    public IImmutableList<Coupon.Entities.Coupon> Coupons { get; init; } = ImmutableArray<Coupon.Entities.Coupon>.Empty;
    public IImmutableList<OrderDiscount> OrderDiscounts { get; init; } = ImmutableArray<OrderDiscount>.Empty;

    public DateTimeOffset OrderDate { get; init; } = DateTimeOffset.UtcNow;
    
    public string ConcurrencyToken { get; init; } = string.Empty;

}