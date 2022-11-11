using CaaS.Core.Base;

namespace CaaS.Core.Shop.Entities;

public record ShopAdmin : IEntityBase {
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = string.Empty;
    public string EMail { get; init; } = string.Empty;
    public Guid ShopId { get; init; } = default;
    public string ConcurrencyToken { get; init; } = string.Empty;
}