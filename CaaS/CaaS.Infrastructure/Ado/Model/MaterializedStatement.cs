namespace CaaS.Infrastructure.Ado.Model; 

public record MaterializedStatement(string Sql) {
    public IEnumerable<QueryParameter> Parameters { get; init; } = Enumerable.Empty<QueryParameter>();
}