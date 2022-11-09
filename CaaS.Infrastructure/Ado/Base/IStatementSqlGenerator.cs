using CaaS.Infrastructure.Ado.Model;

namespace CaaS.Infrastructure.Ado.Base; 

public interface IStatementSqlGenerator {
    MaterializedStatements MaterializeStatement(Statement statement);
}