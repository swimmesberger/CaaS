using System.Data.Common;
using CaaS.Core.Repositories.Base;

namespace CaaS.Infrastructure.Ado; 

public sealed class AdoUnitOfWork : IUnitOfWork, IConnectionProvider {
    private readonly IConnectionFactory _dbProviderFactory;

    private DbConnection? _dbConnection;
    private DbTransaction? _dbTransaction;
    private bool _completed;
    public bool Implicit { get; set; }

    public AdoUnitOfWork(IConnectionFactory dbProviderFactory) {
        _dbProviderFactory = dbProviderFactory;
        Implicit = true;
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
            _completed = true;
        }
        Implicit = false;
    }
    
    public async ValueTask DisposeAsync() {
        if (Implicit) {
            await CompleteAsync();
        }
        if (_dbTransaction != null) {
            if (!_completed) {
                await _dbTransaction.RollbackAsync();
            }
            await _dbTransaction.DisposeAsync();
            _dbTransaction = null;
        }
        if (_dbConnection != null) {
            await _dbConnection.DisposeAsync();
            _dbConnection = null;
        }
    }
}