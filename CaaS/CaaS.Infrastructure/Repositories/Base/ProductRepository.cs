using CaaS.Core.Entities.Base;
using CaaS.Core.Exceptions;
using CaaS.Core.Repositories.Base;
using CaaS.Infrastructure.Dao;

namespace CaaS.Infrastructure.Repositories.Base; 

public abstract class AbstractRepository<TData, TDomain> : IRepository<TDomain> where TDomain : class, IEntityBase {
    protected IDao<TData> Dao { get; }

    public AbstractRepository(IDao<TData> dao) {
        Dao = dao;
    }
    
    public async Task<IReadOnlyList<TDomain>> FindAllAsync(CancellationToken cancellationToken = default) 
        => await ConvertToDomain(Dao.FindAllAsync(cancellationToken), cancellationToken);

    public async Task<TDomain?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        var dataModel = await Dao.FindByIdAsync(id, cancellationToken);
        if (dataModel == null) return null;
        return await ConvertToDomain(dataModel, cancellationToken);
    }

    public async Task<List<TDomain>> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default) {
        var dataModel = Dao.FindByIdsAsync(ids, cancellationToken);
        return await ConvertToDomain(dataModel, cancellationToken);
    }

    public async Task<TDomain> AddAsync(TDomain entity, CancellationToken cancellationToken = default) {
        var dataModel = ConvertFromDomain(entity);
        dataModel = await Dao.AddAsync(dataModel, cancellationToken);
        return await ConvertToDomain(dataModel, cancellationToken);
    }

    public async Task<TDomain> UpdateAsync(TDomain entity, CancellationToken cancellationToken = default) {
        var dataModel = await Dao.FindByIdAsync(entity.Id, cancellationToken);
        if (dataModel == null) {
            throw new CaasUpdateDbException($"Can't update entity {entity.Id} - entity not found");
        }
        dataModel = ApplyDomainModel(dataModel, entity);
        dataModel = await Dao.UpdateAsync(dataModel, cancellationToken);
        return await ConvertToDomain(dataModel, cancellationToken);
    }

    public async Task DeleteAsync(TDomain entity, CancellationToken cancellationToken = default)
        => await Dao.DeleteAsync(ConvertFromDomain(entity), cancellationToken);

    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
        => await Dao.CountAsync(cancellationToken);

    protected ValueTask<TDomain> ConvertToDomain(TData dataModel) => ConvertToDomain(dataModel, default);

    protected abstract TData ApplyDomainModel(TData dataModel, TDomain domainModel);
    
    protected abstract TData ConvertFromDomain(TDomain domainModel);

    protected abstract ValueTask<TDomain> ConvertToDomain(TData dataModel, CancellationToken cancellationToken);

    protected abstract Task<List<TDomain>> ConvertToDomain(IAsyncEnumerable<TData> dataModels, 
            CancellationToken cancellationToken = default);
}