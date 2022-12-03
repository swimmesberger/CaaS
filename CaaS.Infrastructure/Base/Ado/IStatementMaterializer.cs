using CaaS.Infrastructure.Base.Ado.Query;

namespace CaaS.Infrastructure.Base.Ado; 

public interface IStatementMaterializer {
    MaterializedStatements MaterializeBatch(StatementBatch statement);
    MaterializedStatement<T> MaterializeStatement<T>(Statement<T> statement);
}