using System.Data.Common;

namespace CaaS.Infrastructure.Ado; 

public interface IConnectionFactory {
    DbConnection CreateDbConnection();
}