using System.Data.Common;
using CaaS.Core.Repositories.Base;
using CaaS.Infrastructure.Ado.Base;

namespace CaaS.Infrastructure.Ado; 

public sealed class AdoUnitOfWork : IUnitOfWork, IConnectionProvider {
    private readonly IConnectionFactory _dbProviderFactory;
    private readonly bool _transactional;
    
    private DbConnection? _dbConnection;
    private DbTransaction? _dbTransaction;

    public DbTransaction? CurrentTransaction => _dbTransaction;
    public IConnectionFactory Factory => _dbProviderFactory;

    internal bool Disposed { get; private set; }

    public AdoUnitOfWork(IConnectionFactory dbProviderFactory, bool transactional = true) {
        _dbProviderFactory = dbProviderFactory;
        _transactional = transactional;
    }

    public async Task<DbConnection> GetDbConnectionAsync(CancellationToken cancellationToken = default) {
        if (_dbConnection == null) {
            _dbConnection = _dbProviderFactory.CreateConnection();
            await _dbConnection.OpenAsync(cancellationToken);
        }
        if (_transactional && _dbTransaction == null) {
            _dbTransaction = await _dbConnection.BeginTransactionAsync(cancellationToken);
        }
        return _dbConnection;
    }

    public async Task CompleteAsync(CancellationToken cancellationToken = default) {
        if (_dbTransaction != null) {
            await _dbTransaction.CommitAsync(cancellationToken);
            _dbTransaction = null;
        }
    }
    
    public async ValueTask DisposeAsync() {
        if (_dbTransaction != null) {
            await _dbTransaction.DisposeAsync();
            _dbTransaction = null;
        }
        if (_dbConnection != null) {
            await _dbConnection.DisposeAsync();
            _dbConnection = null;
        }
        Disposed = true;
    }
}