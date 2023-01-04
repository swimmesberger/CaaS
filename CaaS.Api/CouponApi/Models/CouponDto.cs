namespace CaaS.Api.CouponApi.Models; 

public record CouponDto {
    public Guid Id { get; init; }
    public Guid ShopId { get; init; }
    public string Code { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public Guid? OrderId { get; init; } = Guid.Empty;
    public Guid? CartId { get; init; } = Guid.Empty;
    public Guid? CustomerId { get; init; } = Guid.Empty;
    public string ConcurrencyToken { get; init; } = string.Empty;
}