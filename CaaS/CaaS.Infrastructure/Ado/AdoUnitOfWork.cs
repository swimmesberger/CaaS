using System.Data.Common;
using CaaS.Core.Repositories.Base;

namespace CaaS.Infrastructure.Ado; 

public sealed class AdoUnitOfWork : IUnitOfWork, IConnectionProvider {
    private readonly IConnectionFactory _dbProviderFactory;

    private DbConnection? _dbConnection;
    private DbTransaction? _dbTransaction;
    
    internal bool Disposed { get; private set; }

    public AdoUnitOfWork(IConnectionFactory dbProviderFactory) {
        _dbProviderFactory = dbProviderFactory;
    }

    public async Task<DbConnection> GetDbConnectionAsync(bool transactional = false, CancellationToken cancellationToken = default) {
        if (_dbConnection == null) {
            _dbConnection = _dbProviderFactory.CreateDbConnection();
            await _dbConnection.OpenAsync(cancellationToken);
        }
        if (transactional) {
            _dbTransaction ??= await _dbConnection.BeginTransactionAsync(cancellationToken);
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
            await _dbTransaction.RollbackAsync();
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