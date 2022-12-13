namespace CaaS.Api.CouponApi.Models; 

public record CouponDto {
    public Guid Id { get; init; }
    public Guid ShopId { get; init; }
    public decimal Value { get; init; }
    public Guid? OrderId { get; init; }
    public Guid? CartId { get; init; }
    public Guid? CustomerId { get; init; }
    public string ConcurrencyToken { get; init; } = string.Empty;
}