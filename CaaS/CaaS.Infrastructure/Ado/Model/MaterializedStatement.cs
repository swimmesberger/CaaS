namespace CaaS.Infrastructure.Ado.Model; 

public record MaterializedStatement(string Sql) {
    public static readonly MaterializedStatement Empty = new MaterializedStatement(string.Empty);
    
    public IEnumerable<QueryParameter> Parameters { get; init; } = Enumerable.Empty<QueryParameter>();

    public MaterializedStatement Add(MaterializedStatement statement) {
        return new MaterializedStatement($"{Sql};{statement.Sql}") {
            Parameters = statement.Parameters.ToList()
        };
    }
}