namespace CaaS.Core.Tenant; 

public interface ITenantService {
    ValueTask<Tenant> GetTenantAsync(CancellationToken cancellationToken = default);
    
    ValueTask<object> GetTenantIdAsync(CancellationToken cancellationToken = default);
    
    ValueTask<Tenant?> GetTenantOrDefaultAsync(CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<Tenant>> GetTenants(CancellationToken cancellationToken = default);
}