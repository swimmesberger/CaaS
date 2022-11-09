using System.Collections.Immutable;

namespace CaaS.Core.Repositories.Base; 

public static class ImmutableArrayExtensions {
    public static int FindIndex<T>(this ImmutableArray<T> array, Predicate<T> match) {
        var idx = 0;
        foreach (var val in array) {
            if (match.Invoke(val)) return idx;
            idx += 1;
        }
        return -1;
    }
}