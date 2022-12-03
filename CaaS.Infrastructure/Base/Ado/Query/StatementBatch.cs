namespace CaaS.Infrastructure.Base.Ado.Query;

public record StatementBatch(IEnumerable<Statement> Statements) {
    public StatementBatch(params Statement[] statements) : this((IEnumerable<Statement>)statements) {}
}