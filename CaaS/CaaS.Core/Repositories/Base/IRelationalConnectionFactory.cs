using System.Data;
using System.Data.Common;

namespace CaaS.Core.Repositories.Base; 

public interface IRelationalConnectionFactory {
    DbConnection CreateDbConnection();

    DbParameter CreateParameter(string name, DbType type);
}