using System.Collections.Immutable;

namespace CaaS.Core.Base;

public record CountedResult<T> {
    public IReadOnlyCollection<T> Items { get; init; } = ImmutableArray<T>.Empty;
    
    public long TotalCount { get; init; }
}