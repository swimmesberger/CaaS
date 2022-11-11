using CaaS.Core.Base;
using CaaS.Core.Exceptions;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.DataModel;

namespace CaaS.Infrastructure.Base.Repository; 

public class CrudRepository<TData, TDomain> : CrudReadRepository<TData, TDomain> 
        where TDomain : class, IEntityBase 
        where TData : IDataModelBase {
    internal new IDomainModelConverter<TData, TDomain> Converter => (IDomainModelConverter<TData, TDomain>)base.Converter;
    
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    public CrudRepository(IDao<TData> dao, IDomainModelConverter<TData, TDomain> converter) : base(dao, converter) { }

    public async Task<TDomain> AddAsync(TDomain entity, CancellationToken cancellationToken = default) {
        var dataModel = Converter.ConvertFromDomain(entity);
        dataModel = await Dao.AddAsync(dataModel, cancellationToken);
        return await Converter.ConvertToDomain(dataModel, cancellationToken);
    }

    public async Task<TDomain> UpdateAsync(TDomain entity, CancellationToken cancellationToken = default) {
        var dataModel = await Dao.FindByIdAsync(entity.Id, cancellationToken);
        if (dataModel == null) {
            throw new CaasUpdateDbException($"Can't update entity {entity.Id} - entity not found");
        }
        return await UpdateImplAsync(dataModel, entity, cancellationToken);
    }

    public async Task<TDomain> UpdateAsync(TDomain oldEntity, TDomain newEntity, CancellationToken cancellationToken = default) {
        var dataModel = Converter.ConvertFromDomain(oldEntity);
        return await UpdateImplAsync(dataModel, newEntity, cancellationToken);
    }
    
    private async Task<TDomain> UpdateImplAsync(TData dataModel, TDomain entity, CancellationToken cancellationToken = default) {
        dataModel = Converter.ApplyDomainModel(dataModel, entity);
        dataModel = await Dao.UpdateAsync(dataModel, cancellationToken);
        return await Converter.ConvertToDomain(dataModel, cancellationToken);
    }

    public async Task DeleteAsync(TDomain entity, CancellationToken cancellationToken = default)
        => await Dao.DeleteAsync(Converter.ConvertFromDomain(entity), cancellationToken);
}