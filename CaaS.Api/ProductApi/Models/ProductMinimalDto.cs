namespace CaaS.Api.ProductApi.Models; 

public record ProductMinimalDto {
    public static readonly ProductMinimalDto Empty = new ProductMinimalDto();
    
    public Guid Id { get; init; }
    public Guid ShopId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string ImageSrc { get; init; } = string.Empty;
    public decimal Price { get; init; }
}