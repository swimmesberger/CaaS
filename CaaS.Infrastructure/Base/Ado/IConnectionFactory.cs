using System.Data.Common;

namespace CaaS.Infrastructure.Base.Ado; 

public interface IConnectionFactory {
    DbConnection CreateConnection();
    
    DbParameter CreateParameter();
}