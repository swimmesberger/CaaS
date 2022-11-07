using CaaS.Core.Entities.Base;

namespace CaaS.Core.Entities;

public record ShopAdmin() : IEntityBase {
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string EMail { get; init; } = string.Empty;
    public Shop Shop { get; init; } = null!;
    public string ConcurrencyToken { get; init; } = string.Empty;
}