using CaaS.Core.Entities.Base;

namespace CaaS.Core.Entities;

public record Shop : Entity {
    public string Name { get; init; } = string.Empty;
}