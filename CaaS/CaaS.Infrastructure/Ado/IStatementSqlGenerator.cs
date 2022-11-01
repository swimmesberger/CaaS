namespace CaaS.Infrastructure.Ado; 

public interface IStatementSqlGenerator {
    MaterializedStatement MaterializeStatement(Statement statement);
}