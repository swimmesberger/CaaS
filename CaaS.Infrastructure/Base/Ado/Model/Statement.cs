using CaaS.Infrastructure.Base.Ado.Impl;

namespace CaaS.Infrastructure.Base.Ado.Model;

public record Statement {
    public static readonly Statement Empty = new Statement(StatementType.Find, EmptyStatementSqlGenerator.Instance);
    
    private readonly IStatementSqlGenerator _sqlGenerator;
    
    public StatementType Type { get; }
    
    public StatementParameters Parameters { get; init; } = StatementParameters.Empty;

    public Statement(StatementType type, IStatementSqlGenerator sqlGenerator) {
        Type = type;
        _sqlGenerator = sqlGenerator;
    }

    public Statement AddParameters(StatementParameters parameters) {
        return this with {
            Parameters = Parameters.Add(parameters)
        };
    }

    public Statement AddWhereParameter(string name, object value) {
        return AddParameters(StatementParameters.CreateWhere(QueryParameter.From(name, value)));
    }

    public MaterializedStatements Materialize() => _sqlGenerator.MaterializeStatement(this);
}