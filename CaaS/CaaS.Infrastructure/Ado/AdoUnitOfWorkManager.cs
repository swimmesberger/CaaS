using System.Data.Common;
using CaaS.Core.Repositories.Base;

namespace CaaS.Infrastructure.Ado; 

public class AdoUnitOfWorkManager : IUnitOfWorkManager, IHasConnectionProvider {
    private readonly IConnectionFactory _dbProviderFactory;

    private AdoUnitOfWork? _current;
    public IUnitOfWork? Current => _current;
    
    public IConnectionProvider ConnectionProvider {
        get {
            if (_current != null && !_current.Disposed) {
                return new ShieldedConnectionProvider(_current);
            }
            return new AdoUnitOfWork(_dbProviderFactory, transactional: false);
        }
    }

    public AdoUnitOfWorkManager(IConnectionFactory dbProviderFactory) {
        _dbProviderFactory = dbProviderFactory;
    }

    public IUnitOfWork Begin() {
        if (_current != null && !_current.Disposed) {
            return _current;
        }
        var uow = new AdoUnitOfWork(_dbProviderFactory, transactional: true);
        _current = uow;
        return uow;
    }

    private class ShieldedConnectionProvider : IConnectionProvider {
        private readonly IConnectionProvider _connectionProvider;

        public ShieldedConnectionProvider(IConnectionProvider connectionProvider) {
            _connectionProvider = connectionProvider;
        }

        public Task<DbConnection> GetDbConnectionAsync(CancellationToken cancellationToken = default) =>
                _connectionProvider.GetDbConnectionAsync(cancellationToken);
        
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}