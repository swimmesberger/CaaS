using CaaS.Core.Entities;
using CaaS.Core.Tenant;
using CaaS.Infrastructure.Ado;
using CaaS.Infrastructure.Repositories.Base;
using CaaS.Infrastructure.Repositories.Base.Mapping;

namespace CaaS.Infrastructure.Repositories; 

public class ProductRepository : TenantRepository<Product> {
    public ProductRepository(IStatementExecutor statementExecutor, IStatementGenerator<Product> statementGenerator,
            ITenantService tenantService) : base(statementExecutor, statementGenerator, tenantService) { }
}