using System.Collections.Immutable;
using CaaS.Core.Entities.Base;

namespace CaaS.Core.Entities;

public record Order : IEntityBase {
    public Guid Id { get; init; } = Guid.NewGuid();
    public int OrderNumber { get; init; }
    public Shop Shop { get; init; } = null!;
    public Customer Customer { get; init; } = null!;
    
    public ImmutableList<OrderItem> Products { get; init; } = ImmutableList<OrderItem>.Empty;
    public ImmutableList<Coupon> Coupons { get; init; } = ImmutableList<Coupon>.Empty;
    public ImmutableList<decimal> OrderDicounts { get; init; } = ImmutableList<decimal>.Empty;
    
    public DateTime OrderDate { get; init; }

    public string ConcurrencyToken { get; init; } = string.Empty;

}