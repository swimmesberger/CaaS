namespace CaaS.Core.Entities.Base; 

public record TenantEntity : Entity, IHasTenant {
    public string TenantId { get; init; } = string.Empty;
}