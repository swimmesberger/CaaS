using CaaS.Core.Base;

namespace CaaS.Core.ShopAggregate;

public record Shop : IEntityBase {
    public const int DefaultCartLifetimeMinutes = 120;
    public static readonly Shop Empty = new Shop() { Id = Guid.Empty };
    
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = string.Empty;
    public int CartLifetimeMinutes { get; init; } = DefaultCartLifetimeMinutes;
    public ShopAdmin ShopAdmin { get; init; } = null!;
    public string ConcurrencyToken { get; init; } = string.Empty;
}