namespace CaaS.Core.CouponAggregate;

public record CouponQuery {
    public Guid? CartId { get; init; }
    public Guid? OrderId { get; init; }
    public Guid? CustomerId { get; init; }
    public string? Code { get; init; } = null;
}