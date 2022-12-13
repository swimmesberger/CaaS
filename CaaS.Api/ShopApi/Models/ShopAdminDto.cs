namespace CaaS.Api.ShopApi.Models; 

public record ShopAdminDto {
    public static readonly ShopAdminDto Empty = new ShopAdminDto();
    
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string EMail { get; init; } = string.Empty;
    public Guid ShopId { get; init; }
    public string ConcurrencyToken { get; init; } = string.Empty;
}