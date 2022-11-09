using System.Collections.Immutable;
using CaaS.Core;
using CaaS.Core.Entities.Base;
using CaaS.Core.Repositories.Base;
using CaaS.Infrastructure.Ado.Base;

namespace CaaS.Infrastructure.Repositories.Base; 

public static class RepositoryExtensions {
    public static async Task ApplyAsync<T>(this ICrudBulkWriteRepository<T> writeRepository, ChangeTracker<T> changeTracker, 
        CancellationToken cancellationToken = default) where T : IEntityBase {
        await writeRepository.AddAsync(changeTracker.AddedItems, cancellationToken);
        await writeRepository.UpdateAsync(changeTracker.UpdatedItems, cancellationToken);
        await writeRepository.DeleteAsync(changeTracker.RemovedItems, cancellationToken);
    }
    
    public static async Task ApplyAsync<T>(this IDao<T> dao, ChangeTracker<T> changeTracker, 
        CancellationToken cancellationToken = default) where T : IHasId {
        await dao.AddAsync(changeTracker.AddedItems.ToImmutableArray(), cancellationToken);
        await dao.UpdateAsync(changeTracker.UpdatedItems.ToImmutableArray(), cancellationToken);
        await dao.DeleteAsync(changeTracker.RemovedItems.ToImmutableArray(), cancellationToken);
    }
    
    public static async Task ApplyAsync<T>(this IDao<T> writeRepository, IEnumerable<T> oldItems, 
        IReadOnlyCollection<T> newItems, CancellationToken cancellationToken = default) where T : IHasId {
        var changeTracker = ChangeTracker.CreateDiff(oldItems, newItems);
        await writeRepository.ApplyAsync(changeTracker, cancellationToken);
    }

    public static async Task ApplyAsync<T>(this ICrudBulkWriteRepository<T> writeRepository, IEnumerable<T> oldItems, 
        IReadOnlyCollection<T> newItems, CancellationToken cancellationToken = default) where T : IEntityBase {
        var changeTracker = ChangeTracker.CreateDiff(oldItems, newItems);
        await writeRepository.ApplyAsync(changeTracker, cancellationToken);
    }
}