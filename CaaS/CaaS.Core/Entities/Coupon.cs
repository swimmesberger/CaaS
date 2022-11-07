using CaaS.Core.Entities.Base;

namespace CaaS.Core.Entities; 

public record Coupon : IEntityBase {
    public Guid Id { get; init; }
    public string DiscountName { get; init; }
    public Shop Shop { get; init; } = null!;
    public decimal Value { get; init; }
    public Order Order { get; init; } = null!;
    public Cart Cart { get; init; } = null!;
    public Customer RedeemedBy { get; init; } = null!;
    public string ConcurrencyToken { get; init; }
}