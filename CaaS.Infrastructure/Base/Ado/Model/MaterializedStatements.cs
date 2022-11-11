using System.Collections.Immutable;

namespace CaaS.Infrastructure.Base.Ado.Model;

public record MaterializedStatements {
    public static readonly MaterializedStatements Empty = new MaterializedStatements();
    
    public IImmutableList<MaterializedStatement> Statements { get; init; } = ImmutableArray<MaterializedStatement>.Empty;

    public int Count => Statements.Count;

    public MaterializedStatements(ImmutableArray<MaterializedStatement> statements) {
        Statements = statements;
    }
    
    public MaterializedStatements(IEnumerable<MaterializedStatement> statements) : this(statements.ToImmutableArray()) { }
    
    public MaterializedStatements(MaterializedStatement statement) : this(ImmutableArray.Create(statement)) { }

    public MaterializedStatements() { }

    public MaterializedStatements Add(MaterializedStatement statement) => new MaterializedStatements(Statements.Add(statement));
}