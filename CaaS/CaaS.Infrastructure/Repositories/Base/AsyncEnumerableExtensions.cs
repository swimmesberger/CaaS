using System.Collections.Immutable;

namespace CaaS.Infrastructure.Repositories.Base; 

public static class AsyncEnumerableExtensions {
    public static ValueTask<ImmutableList<TSource>> ToImmutableListAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default) {
        if (source == null)
            throw new ArgumentException("Source null", nameof(source));

        return source.ToCollection(
            ImmutableList.CreateBuilder<TSource>(),
            static builder => builder.ToImmutable(),
            cancellationToken);
    }
    
    public static ValueTask<ImmutableArray<TSource>> ToImmutableArrayAsync<TSource>(
        this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default) {
        if (source == null)
            throw new ArgumentException("Source null", nameof(source));

        return source.ToCollection(
            ImmutableArray.CreateBuilder<TSource>(),
            static builder => builder.ToImmutable(),
            cancellationToken);
    }
    
    public static async ValueTask<TResult> ToCollection<TSource, TCollection, TResult>(
        this IAsyncEnumerable<TSource> source,
        TCollection collection,
        Func<TCollection, TResult> resultSelector,
        CancellationToken cancellationToken)
        where TCollection : ICollection<TSource> {
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false)) {
            collection.Add(item);
        }
        return resultSelector(collection);
    }
}