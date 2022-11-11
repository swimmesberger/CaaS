using CaaS.Infrastructure.Base.Ado.Model;

namespace CaaS.Infrastructure.Base.Ado.Impl; 

public sealed class EmptyStatementSqlGenerator : IStatementSqlGenerator {
    public static readonly IStatementSqlGenerator Instance = new EmptyStatementSqlGenerator();
    
    private EmptyStatementSqlGenerator() { }
    
    public MaterializedStatements MaterializeStatement(Statement statement) => MaterializedStatements.Empty;
}