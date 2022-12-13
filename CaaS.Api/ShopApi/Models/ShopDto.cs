namespace CaaS.Api.ShopApi.Models; 

public record ShopDto {
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int CartLifetimeMinutes { get; init; }
    public ShopAdminDto ShopAdmin { get; init; } = ShopAdminDto.Empty;
    public string ConcurrencyToken { get; init; } = string.Empty;
} 