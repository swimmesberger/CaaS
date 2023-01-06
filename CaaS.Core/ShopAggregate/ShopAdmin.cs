using CaaS.Core.Base;

namespace CaaS.Core.ShopAggregate;

public record ShopAdmin : IEntityBase {
    public static readonly ShopAdmin Empty = new ShopAdmin() { Id = Guid.Empty };
    
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = string.Empty;
    public string EMail { get; init; } = string.Empty;
    public Guid ShopId { get; init; } = default;
    public string ConcurrencyToken { get; init; } = string.Empty;
}