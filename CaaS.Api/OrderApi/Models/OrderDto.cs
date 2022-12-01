using System.Collections.Immutable;
using CaaS.Api.CouponApi.Models;
using CaaS.Api.DiscountApi.Models;
using CaaS.Core.OrderAggregate;

namespace CaaS.Api.OrderApi.Models; 

public record OrderDto {
    public Guid Id { get; init; }
    public Guid ShopId { get; init; }
    public int OrderNumber { get; init; }
    public Guid CustomerId { get; init; }
    
    public IReadOnlyList<OrderItemDto> Items { get; init; } = ImmutableArray<OrderItemDto>.Empty;
    public IReadOnlyList<CouponDto> Coupons { get; init; } = ImmutableArray<CouponDto>.Empty;
    public IReadOnlyList<DiscountDto> OrderDiscounts { get; init; } = ImmutableArray<DiscountDto>.Empty;
    
    public Address BillingAddress { get; init; } = Address.Empty;

    public DateTimeOffset OrderDate { get; init; }
}