using CaaS.Infrastructure.Ado.Model;

namespace CaaS.Infrastructure.Ado.Base; 

public interface IStatementSqlGenerator {
    MaterializedStatement MaterializeStatement(Statement statement);
}