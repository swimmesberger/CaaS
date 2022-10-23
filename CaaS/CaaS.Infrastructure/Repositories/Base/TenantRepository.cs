using CaaS.Core.Entities.Base;
using CaaS.Core.Tenant;
using CaaS.Infrastructure.Ado;
using CaaS.Infrastructure.Repositories.Base.Mapping;

namespace CaaS.Infrastructure.Repositories.Base; 

public abstract class TenantRepository<T> : Repository<T> where T : TenantEntity, new() {
    private readonly ITenantService _tenantService;
    
    public TenantRepository(IQueryExecutor queryExecutor, 
            IPropertyMappingProvider<T> propertyMappingProvider, 
            ITenantService tenantService) : base(queryExecutor, propertyMappingProvider) {
        _tenantService = tenantService;
    }

    protected override async Task<Statement> PostProcessStatement(Statement statement, CancellationToken cancellationToken = default) {
        var tenantId = await _tenantService.GetTenantAsync(cancellationToken);
        var paramsList = statement.Parameters?.ToList() ?? new List<QueryParameter>();
        var tenantIdColumnName = PropertyMapping.MapProperty(nameof(IHasTenant.TenantId));
        paramsList.Add(new QueryParameter(tenantIdColumnName, tenantId));
        statement = new Statement(statement.Sql + $" AND {tenantIdColumnName} = @{tenantIdColumnName}", paramsList);
        return statement;
    }
}