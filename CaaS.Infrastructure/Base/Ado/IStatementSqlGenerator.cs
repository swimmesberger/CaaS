using CaaS.Infrastructure.Base.Ado.Model;

namespace CaaS.Infrastructure.Base.Ado; 

public interface IStatementSqlGenerator {
    MaterializedStatements MaterializeStatement(Statement statement);
}