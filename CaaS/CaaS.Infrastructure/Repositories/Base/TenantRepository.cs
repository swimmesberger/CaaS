using CaaS.Core.Entities.Base;
using CaaS.Core.Tenant;
using CaaS.Infrastructure.Ado;
using CaaS.Infrastructure.Repositories.Base.Mapping;

namespace CaaS.Infrastructure.Repositories.Base; 

public abstract class TenantRepository<T> : Repository<T> where T : TenantEntity, new() {
    private readonly ITenantService _tenantService;
    
    public TenantRepository(IStatementExecutor statementExecutor, 
            IStatementGenerator<T> statementGenerator, 
            ITenantService tenantService) : base(statementExecutor, statementGenerator) {
        _tenantService = tenantService;
    }

    protected override async Task<Statement> PostProcessStatement(Statement statement, CancellationToken cancellationToken = default) {
        var tenantId = await _tenantService.GetTenantAsync(cancellationToken);
        return StatementGenerator.AddFindParameterByProperty(statement, nameof(IHasTenant.TenantId), tenantId);
    }
}