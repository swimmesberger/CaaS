using CaaS.Core.Entities.Base;
using CaaS.Core.Repositories.Base;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;
using CaaS.Infrastructure.DataModel.Base;

namespace CaaS.Infrastructure.Repositories.Base; 

public class CrudReadRepository<TData, TDomain> : IRepository 
        where TDomain : class, IEntityBase
        where TData: IDataModelBase {
    internal IDao<TData> Dao { get; }
    internal IDomainReadModelConverter<TData, TDomain> Converter { get; }

    public CrudReadRepository(IDao<TData> dao, IDomainReadModelConverter<TData, TDomain> converter) {
        Dao = dao;
        Converter = converter;
    }
    
    public async Task<IReadOnlyList<TDomain>> FindAllAsync(CancellationToken cancellationToken = default) 
        => await Converter.ConvertToDomain(Dao.FindBy(PreProcessFindManyParameters(StatementParameters.Empty), cancellationToken), cancellationToken);

    public async Task<TDomain?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        var dataModel = await Dao.FindByIdAsync(id, cancellationToken);
        if (dataModel == null) return null;
        return await Converter.ConvertToDomain(dataModel, cancellationToken);
    }

    public async Task<IReadOnlyList<TDomain>> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default) {
        var dataModel = Dao.FindByIdsAsync(ids, cancellationToken);
        return await Converter.ConvertToDomain(dataModel, cancellationToken);
    }
    
    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
        => await Dao.CountAsync(cancellationToken);

    private StatementParameters PreProcessFindManyParameters(StatementParameters parameters) {
        return parameters.WithOrderBy(Converter.DefaultOrderParameters ?? GetFallbackOrderParameters());
    }

    private static IEnumerable<OrderParameter> GetFallbackOrderParameters() => OrderParameter.From(nameof(IDataModelBase.Id));
}