using CaaS.Core.Entities.Base;

namespace CaaS.Core.Entities;

public record Order : IEntityBase {
    public Guid Id { get; init; }
    public int OrderNumber { get; init; }
    public Shop Shop { get; init; }
    public Customer Customer { get; init; }
    
    public List<OrderItem> Products { get; init; } = new();
    public List<Coupon> Coupons { get; init; } //needed??
    public List<decimal> OrderDicounts { get; init; } = new();
    
    public DateTime OrderDate { get; init; }

    public string ConcurrencyToken { get; init; }
    
}