using CaaS.Core.Entities.Base;
using CaaS.Core.Exceptions;
using CaaS.Core.Repositories.Base;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.DataModel.Base;

namespace CaaS.Infrastructure.Repositories.Base; 

public class CrudRepository<TData, TDomain> : CrudReadRepository<TData, TDomain>, ICrudRepository<TDomain> 
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
        dataModel = Converter.ApplyDomainModel(dataModel, entity);
        dataModel = await Dao.UpdateAsync(dataModel, cancellationToken);
        return await Converter.ConvertToDomain(dataModel, cancellationToken);
    }

    public async Task DeleteAsync(TDomain entity, CancellationToken cancellationToken = default)
        => await Dao.DeleteAsync(Converter.ConvertFromDomain(entity), cancellationToken);
}