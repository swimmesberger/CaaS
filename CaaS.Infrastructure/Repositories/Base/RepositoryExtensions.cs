using CaaS.Core;
using CaaS.Infrastructure.Ado.Base;

namespace CaaS.Infrastructure.Repositories.Base; 

public static class RepositoryExtensions {
    public static async Task ApplyAsync<T>(this IDao<T> dao, ChangeTracker<T> changeTracker, 
        CancellationToken cancellationToken = default) where T : IHasId {
        await dao.AddAsync(changeTracker.AddedItems.ToList(), cancellationToken);
        await dao.UpdateAsync(changeTracker.UpdatedItems.ToList(), cancellationToken);
        await dao.DeleteAsync(changeTracker.RemovedItems.ToList(), cancellationToken);
    }
    
    public static async Task ApplyAsync<T>(this IDao<T> writeRepository, IEnumerable<T> oldItems, 
        IReadOnlyCollection<T> newItems, CancellationToken cancellationToken = default) where T : IHasId {
        var changeTracker = ChangeTracker.CreateDiff(oldItems, newItems);
        await writeRepository.ApplyAsync(changeTracker, cancellationToken);
    }
}