using System.Collections.Immutable;

namespace CaaS.Api.CartApi.Models; 

public record CartForUpdateDto {
    public IReadOnlyList<CartForUpdateItemDto> Items { get; init; } = ImmutableArray<CartForUpdateItemDto>.Empty;
    public IReadOnlyList<CartForUpdateCouponDto> Coupons { get; init; } = ImmutableArray<CartForUpdateCouponDto>.Empty;
    public string ConcurrencyToken { get; init; } = string.Empty;
}

public record CartForUpdateItemDto {
    public CartForUpdateCartItemProductDto Product { get; init; } = null!;
    public int Amount { get; init; } 
    public string ConcurrencyToken { get; init; } = string.Empty;
}

public record CartForUpdateCartItemProductDto {
    public Guid Id { get; init; }
}

public record CartForUpdateCouponDto {
    public string Code { get; init; } = string.Empty;
}