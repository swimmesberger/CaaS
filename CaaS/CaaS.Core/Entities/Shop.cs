using CaaS.Core.Entities.Base;

namespace CaaS.Core.Entities;

public record Shop : IEntityBase {
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    
    public string ConcurrencyToken { get; init; } = string.Empty;
}