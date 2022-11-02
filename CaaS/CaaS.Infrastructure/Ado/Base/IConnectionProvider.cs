using System.Data.Common;

namespace CaaS.Infrastructure.Ado.Base; 

public interface IConnectionProvider : IAsyncDisposable {
    Task<DbConnection> GetDbConnectionAsync(CancellationToken cancellationToken = default);
}