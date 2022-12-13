namespace CaaS.Api.ProductApi.Models; 

public record ProductDto {
    public static readonly ProductDto Empty = new ProductDto();
    
    public Guid Id { get; init; }
    public Guid ShopId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string DownloadLink { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public bool Deleted { get; init; } = false;
    public string ConcurrencyToken { get; init; } = string.Empty;
}