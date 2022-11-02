using System.Data.Common;

namespace CaaS.Infrastructure.Ado.Base; 

public interface IConnectionFactory {
    DbConnection CreateDbConnection();
}