using System.Collections.Immutable;
using CaaS.Api.DiscountApi.Models;
using CaaS.Api.ProductApi.Models;

namespace CaaS.Api.OrderApi.Models; 

public record OrderItemDto {
    public Guid Id { get; init;  }
    
    public ProductDto Product { get; init; } = ProductDto.Empty;
    public Guid ShopId { get; init; }
    public Guid OrderId { get; init; }
    public int Amount { get; init; }
    
    public IReadOnlyList<DiscountDto> OrderItemDiscounts { get; init; } = ImmutableArray<DiscountDto>.Empty;
    public decimal PricePerPiece { get; init; }
}