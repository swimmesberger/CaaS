using System.Data.Common;

namespace CaaS.Infrastructure.Base.Ado; 

public interface IConnectionProvider : IAsyncDisposable {
    DbTransaction? CurrentTransaction { get; }

    IConnectionFactory Factory { get; }
    
    Task<DbConnection> GetDbConnectionAsync(CancellationToken cancellationToken = default);
}