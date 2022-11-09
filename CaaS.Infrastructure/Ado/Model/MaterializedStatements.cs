using System.Collections.Immutable;

namespace CaaS.Infrastructure.Ado.Model;

public record MaterializedStatements {
    public static readonly MaterializedStatements Empty = new MaterializedStatements();
    
    public ImmutableArray<MaterializedStatement> Statements { get; init; } = ImmutableArray<MaterializedStatement>.Empty;

    public int Count => Statements.Length;

    public MaterializedStatements(ImmutableArray<MaterializedStatement> statements) {
        Statements = statements;
    }
    
    public MaterializedStatements(IEnumerable<MaterializedStatement> statements) : this(statements.ToImmutableArray()) { }
    
    public MaterializedStatements(MaterializedStatement statement) : this(ImmutableList.Create(statement)) { }

    public MaterializedStatements() { }

    public MaterializedStatements Add(MaterializedStatement statement) => new MaterializedStatements(Statements.Add(statement));
}