namespace CaaS.Infrastructure.Ado; 

public record MaterializedStatement(string Sql) {
    public IEnumerable<QueryParameter> Parameters { get; init; } = Enumerable.Empty<QueryParameter>();
}