using CaaS.Core.Repositories.Base;

namespace CaaS.Infrastructure.Ado; 

public class AdoUnitOfWorkManager : IUnitOfWorkManager {
    private readonly IConnectionFactory _dbProviderFactory;
    
    public IUnitOfWork? Current { get; private set; }
    
    public AdoUnitOfWorkManager(IConnectionFactory dbProviderFactory) {
        _dbProviderFactory = dbProviderFactory;
    }

    public IUnitOfWork Begin() {
        if (Current != null) {
            return Current;
        }
        var uow = new AdoUnitOfWork(_dbProviderFactory);
        uow.Implicit = false;
        Current = uow;
        return uow;
    }
}