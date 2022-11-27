﻿using CaaS.Core.Base;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Model;

namespace CaaS.Infrastructure.Base.Repository; 

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

    public Task<PagedResult<TDomain>> FindAllPagedAsync(PaginationToken? paginationToken = null,
        CancellationToken cancellationToken = default) {
        return FindByPagedAsync(StatementParameters.Empty, null, paginationToken, cancellationToken);
    }
    
    public async Task<TDomain?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        var dataModel = await Dao.FindByIdAsync(id, cancellationToken);
        if (dataModel == null) return null;
        return await Converter.ConvertToDomain(dataModel, cancellationToken);
    }

    public async Task<IReadOnlyList<TDomain>> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default) {
        var dataModel = Dao.FindByIdsAsync(ids, cancellationToken);
        return await Converter.ConvertToDomain(dataModel, cancellationToken);
    }

    protected async Task<PagedResult<TDomain>> FindByPagedAsync(StatementParameters parameters, long? pageSize = null, PaginationToken? paginationToken = null, 
        CancellationToken cancellationToken = default) {
        if (!parameters.OrderBy.Any()) {
            parameters = parameters.WithOrderBy(GetPaginationOrderByParameters());
        }
        var pagesModels = await Dao.FindByPagedAsync(parameters, pageSize, paginationToken, cancellationToken);
        return await pagesModels.MapAsync(items => 
            Converter.ConvertToDomain(items.ToAsyncEnumerable(), cancellationToken));
    }

    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
        => await Dao.CountAsync(cancellationToken: cancellationToken);

    protected IEnumerable<OrderParameter> GetPaginationOrderByParameters() {
        var orderParameters = Converter.DefaultOrderParameters;
        orderParameters = orderParameters == null ? GetFallbackOrderParameters() : orderParameters.Concat(GetFallbackOrderParameters());
        return orderParameters;
    }

    private StatementParameters PreProcessFindManyParameters(StatementParameters parameters) {
        return parameters.WithOrderBy(Converter.DefaultOrderParameters ?? GetFallbackOrderParameters());
    }

    private static IEnumerable<OrderParameter> GetFallbackOrderParameters() => OrderParameter.From(nameof(IDataModelBase.Id));
}