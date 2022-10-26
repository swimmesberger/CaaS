using CaaS.Core.Entities;
using CaaS.Core.Repositories;
using CaaS.Infrastructure.Ado;
using CaaS.Infrastructure.Repositories.Base;
using CaaS.Infrastructure.Repositories.Base.Mapping;

namespace CaaS.Infrastructure.Repositories; 

public class ShopRepository : Repository<Shop>, IShopRepository {
    public ShopRepository(IStatementExecutor statementExecutor, 
            IStatementGenerator<Shop> statementGenerator) : base(statementExecutor, statementGenerator) { }

    public async Task<Shop?> FindByNameAsync(string name, CancellationToken cancellationToken = default) {
        return (await QueryByPropertyAsync(nameof(Shop.Name), name, cancellationToken)).FirstOrDefault();
    }
}