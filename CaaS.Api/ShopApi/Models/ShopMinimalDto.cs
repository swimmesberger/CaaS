namespace CaaS.Api.ShopApi.Models;

public record ShopMinimalDto {
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}