using System.Collections.Immutable;
using CaaS.Core.Base;
using CaaS.Core.CouponAggregate;
using CaaS.Core.CustomerAggregate;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.OrderAggregate;

public record Order : IEntityBase {
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid ShopId { get; init; }
    public int OrderNumber { get; init; }
    public Customer Customer { get; init; } = null!;
    
    public IImmutableList<OrderItem> Items { get; init; } = ImmutableArray<OrderItem>.Empty;
    public IImmutableList<Coupon> Coupons { get; init; } = ImmutableArray<Coupon>.Empty;
    public IImmutableList<Discount> OrderDiscounts { get; init; } = ImmutableArray<Discount>.Empty;
    public Address Address { get; init; } = Address.Empty;
    public DateTimeOffset OrderDate { get; init; } = DateTimeOffsetProvider.GetNow();
    
    public string ConcurrencyToken { get; init; } = string.Empty;

}