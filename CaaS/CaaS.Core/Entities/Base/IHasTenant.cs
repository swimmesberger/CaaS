namespace CaaS.Core.Entities.Base; 

public interface IHasTenant {
    string TenantId { get; }
}