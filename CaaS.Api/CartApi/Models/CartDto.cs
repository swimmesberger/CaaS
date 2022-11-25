using System.Collections.Immutable;
using CaaS.Api.CouponApi.Models;
using CaaS.Api.CustomerApi.Models;
using CaaS.Api.DiscountApi.Models;

namespace CaaS.Api.CartApi.Models; 

public record CartDto {
    public Guid Id { get; init; }
    public Guid ShopId { get; init; }
    public CustomerDto? Customer { get; init; } = CustomerDto.Empty;
    public IReadOnlyList<CartItemDto> Items { get; init; } = ImmutableArray<CartItemDto>.Empty;
    public IReadOnlyList<CouponDto> Coupons { get; init; } = ImmutableArray<CouponDto>.Empty;
    public IReadOnlyList<DiscountDto> CartDiscounts { get; init; } = ImmutableArray<DiscountDto>.Empty;
    public DateTimeOffset LastAccess { get; init; }
}