using CaaS.Core.Entities.Base;

namespace CaaS.Core.Entities; 

public record Product : TenantEntity {
    public string Name { get; init; }
    public decimal Price { get; init; }
}