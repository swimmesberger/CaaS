using CaaS.Core.Entities.Base;

namespace CaaS.Core.Entities; 

public record CartItem : IEntityBase {
    public Guid Id { get; init; } = Guid.NewGuid();
    
    public Product Product { get; init; } = null!;
    public Guid ShopId { get; init; } = default;
    public Guid CartId { get; init; } = default;
    public int Amount { get; init; } = 0;
    
    public string ConcurrencyToken { get; init; } = string.Empty;
}