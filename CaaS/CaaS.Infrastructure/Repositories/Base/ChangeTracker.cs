using System.Collections.Immutable;
using CaaS.Core;

namespace CaaS.Infrastructure.Repositories.Base;

public static class ChangeTracker {
    public static ChangeTracker<T> CreateDiff<T>(IEnumerable<T> oldItems, IReadOnlyCollection<T> newItems) where T: IHasId {
        var oldItemsDict = oldItems.ToImmutableDictionary(i => i.Id);
        var addedItems = newItems.Where(newItem => !oldItemsDict.ContainsKey(newItem.Id));
        var updatedItems = newItems.Where(newItem => {
            if (!oldItemsDict.TryGetValue(newItem.Id, out var oldItem)) {
                return false;
            }
            return !newItem.Equals(oldItem);
        });
        var removedItems = oldItemsDict.Values.Except(newItems, EntityBaseIdComparer<T>.Instance);
        return new ChangeTracker<T>(addedItems, updatedItems, removedItems);
    }
    
    private class EntityBaseIdComparer<T> : IEqualityComparer<T> where T: IHasId {
        public static readonly EntityBaseIdComparer<T> Instance = new EntityBaseIdComparer<T>();
        
        public bool Equals(T? x, T? y) {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Id.Equals(y.Id);
        }
        
        public int GetHashCode(T obj) => obj.Id.GetHashCode();
    }
}

public record ChangeTracker<T>(
    IEnumerable<T> AddedItems, 
    IEnumerable<T> UpdatedItems, 
    IEnumerable<T> RemovedItems);