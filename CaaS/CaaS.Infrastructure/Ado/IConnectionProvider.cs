using System.Data.Common;

namespace CaaS.Infrastructure.Ado; 

public interface IConnectionProvider {
    Task<DbConnection> GetDbConnectionAsync(bool transactional = false, CancellationToken cancellationToken = default);
}