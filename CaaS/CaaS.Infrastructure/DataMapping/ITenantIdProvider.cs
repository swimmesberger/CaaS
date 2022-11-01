namespace CaaS.Infrastructure.DataMapping; 

public interface ITenantIdProvider<in T> {
    string TenantIdPropertyName { get; }
    object GetTenantId(T record);
}