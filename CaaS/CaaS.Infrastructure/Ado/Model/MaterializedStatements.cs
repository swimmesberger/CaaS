using System.Collections.Immutable;

namespace CaaS.Infrastructure.Ado.Model;

public record MaterializedStatements {
    public static readonly MaterializedStatements Empty = new MaterializedStatements();
    
    public ImmutableList<MaterializedStatement> Statements { get; init; } = ImmutableList<MaterializedStatement>.Empty;

    public int Count => Statements.Count;

    public MaterializedStatements(ImmutableList<MaterializedStatement> statements) {
        Statements = statements;
    }
    
    public MaterializedStatements(IEnumerable<MaterializedStatement> statements) : this(statements.ToImmutableList()) { }
    
    public MaterializedStatements(MaterializedStatement statement) : this(ImmutableList.Create(statement)) { }

    public MaterializedStatements() { }

    public MaterializedStatements Add(MaterializedStatement statement) => new MaterializedStatements(Statements.Add(statement));
}