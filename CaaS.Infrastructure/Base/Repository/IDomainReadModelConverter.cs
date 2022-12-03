using CaaS.Infrastructure.Base.Ado.Query.Parameters;

namespace CaaS.Infrastructure.Base.Repository; 

public interface IDomainReadModelConverter<in TData, TDomain> {
    OrderParameters DefaultOrderParameters => OrderParameters.Empty;

    ValueTask<TDomain> ConvertToDomain(TData dataModel, CancellationToken cancellationToken);

    Task<IReadOnlyList<TDomain>> ConvertToDomain(IAsyncEnumerable<TData> dataModels, CancellationToken cancellationToken = default);
}