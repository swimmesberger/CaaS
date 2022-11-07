using CaaS.Infrastructure.Ado.Model;

namespace CaaS.Infrastructure.Ado.Base; 

public sealed class EmptyStatementSqlGenerator : IStatementSqlGenerator {
    public static readonly IStatementSqlGenerator Instance = new EmptyStatementSqlGenerator();
    
    private EmptyStatementSqlGenerator() { }
    
    public MaterializedStatements MaterializeStatement(Statement statement) => MaterializedStatements.Empty;
}