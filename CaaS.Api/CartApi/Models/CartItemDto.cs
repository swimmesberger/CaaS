using System.Collections.Immutable;
using CaaS.Api.DiscountApi.Models;
using CaaS.Api.ProductApi.Models;

namespace CaaS.Api.CartApi.Models; 

public record CartItemDto {
    public Guid Id { get; init; }
    
    public ProductDto Product { get; init; } = ProductDto.Empty;
    public Guid ShopId { get; init; }
    public Guid CartId { get; init; } 
    public int Amount { get; init; } 
    public IReadOnlyList<DiscountDto> CartItemDiscounts { get; init; } = ImmutableArray<DiscountDto>.Empty;
    public decimal TotalPrice { get; init; }
    public string ConcurrencyToken { get; init; } = string.Empty;
}