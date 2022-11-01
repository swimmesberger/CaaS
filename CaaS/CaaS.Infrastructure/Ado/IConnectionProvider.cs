using System.Data.Common;

namespace CaaS.Infrastructure.Ado; 

public interface IConnectionProvider : IAsyncDisposable {
    Task<DbConnection> GetDbConnectionAsync(CancellationToken cancellationToken = default);
}