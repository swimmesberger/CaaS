using System.Collections.Immutable;
using CaaS.Infrastructure.Base.Ado.Query;

namespace CaaS.Infrastructure.Base.Mapping;

public record PropertyMetadata {
    public string PropertyName { get; init; } = string.Empty;
    public string ColumnName { get; init; } = string.Empty;
    public string TypeName { get; init; } = string.Empty;
    public bool IsJson { get; init; } = false;
    public IImmutableSet<StatementType>? IgnoredStatementTypes { get; init; }
    
}