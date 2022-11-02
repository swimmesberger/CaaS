using CaaS.Infrastructure.Ado.Base;

namespace CaaS.Infrastructure.Ado.Model;

public record Statement {
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
        return AddParameters(new StatementParameters() { Where = new List<QueryParameter>() { new QueryParameter(name, value) } });
    }

    public MaterializedStatement Materialize() => _sqlGenerator.MaterializeStatement(this);
}