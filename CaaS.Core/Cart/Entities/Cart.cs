using System.Collections.Immutable;
using CaaS.Core.Base;

namespace CaaS.Core.Cart.Entities; 

public record Cart : IEntityBase {
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid ShopId { get; init; } = default;
    public Customer.Entities.Customer? Customer { get; init; }
    public IImmutableList<CartItem> Items { get; init; } = ImmutableArray<CartItem>.Empty;
    public DateTimeOffset LastAccess { get; init; } = DateTimeOffset.UtcNow;
    public string ConcurrencyToken { get; init; } = string.Empty;
}