namespace CaaS.Infrastructure.DataMapping.Base; 

public interface ITenantIdProvider<in T> {
    string TenantIdPropertyName { get; }
    object GetTenantId(T record);
}