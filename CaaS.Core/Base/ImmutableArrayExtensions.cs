using System.Collections.Immutable;

namespace CaaS.Core.Base; 

public static class ImmutableArrayExtensions {
    public static int FindIndex<T>(this IImmutableList<T> list, Predicate<T> match) {
        var idx = 0;
        foreach (var val in list) {
            if (match.Invoke(val)) return idx;
            idx += 1;
        }
        return -1;
    }
}