using System.Collections;
using System.Collections.Immutable;

namespace CaaS.Core.Base;

public record CountedResult<T> : IEnumerable<T> {
    public IEnumerable<T> Items { protected get; init; } = ImmutableArray<T>.Empty;
    
    public long TotalCount { get; init; }
    
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}