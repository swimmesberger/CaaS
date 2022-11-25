namespace CaaS.Api.DiscountApi.Models; 

public record DiscountDto {
    public Guid Id { get; init; }
    public string DiscountName { get; init; } = string.Empty;
    public decimal DiscountValue { get; init; }
    public Guid ShopId { get; init; }
    public Guid ParentId { get; init; }
}