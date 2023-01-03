using System.Collections.Immutable;

namespace CaaS.Core.Base; 

public static class CollectionsExtensions {
    public static int FindIndex<T>(this IImmutableList<T> list, Predicate<T> match) {
        var idx = 0;
        foreach (var val in list) {
            if (match.Invoke(val)) return idx;
            idx += 1;
        }
        return -1;
    }
    
    public static int FindIndex<T>(this IReadOnlyList<T> list, Predicate<T> match) {
        var idx = 0;
        foreach (var val in list) {
            if (match.Invoke(val)) return idx;
            idx += 1;
        }
        return -1;
    }
    
    public static IReadOnlyList<T> Add<T>(this IReadOnlyList<T> list, T item) {
        if (list is IImmutableList<T> immutableList) {
            return immutableList.Add(item);
        }
        if (list is ImmutableArray<T> immutableArray) {
            return immutableArray.Add(item);
        }
        return ImmutableList.CreateRange(list).Add(item);
    }
    
    public static IReadOnlyList<T> AddRange<T>(this IReadOnlyList<T> list, IEnumerable<T> item) {
        if (list is IImmutableList<T> immutableList) {
            return immutableList.AddRange(item);
        }
        if (list is ImmutableArray<T> immutableArray) {
            return immutableArray.AddRange(item);
        }
        return ImmutableList.CreateRange(list).AddRange(item);
    }
}