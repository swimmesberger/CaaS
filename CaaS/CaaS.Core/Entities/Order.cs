using System.Collections.Immutable;
using CaaS.Core.Entities.Base;

namespace CaaS.Core.Entities;

public record Order : IEntityBase {
    public Guid Id { get; init; } = System.Guid.NewGuid();
    public Guid ShopId { get; init; } = default;
    public int OrderNumber { get; init; } = default;
    public Customer Customer { get; init; } = null!;
    
    public ImmutableList<OrderItem> Items { get; init; } = ImmutableList<OrderItem>.Empty;
    public ImmutableList<Coupon> Coupons { get; init; } = ImmutableList<Coupon>.Empty;
    public ImmutableList<OrderDiscount> OrderDiscounts { get; init; } = ImmutableList<OrderDiscount>.Empty;

    public DateTimeOffset OrderDate { get; init; } = DateTimeOffset.UtcNow;
    
    public string ConcurrencyToken { get; init; } = string.Empty;

}