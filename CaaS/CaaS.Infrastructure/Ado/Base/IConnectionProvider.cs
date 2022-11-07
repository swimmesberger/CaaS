using System.Data.Common;

namespace CaaS.Infrastructure.Ado.Base; 

public interface IConnectionProvider : IAsyncDisposable {
    DbTransaction? CurrentTransaction { get; }

    IConnectionFactory Factory { get; }
    
    Task<DbConnection> GetDbConnectionAsync(CancellationToken cancellationToken = default);
}