using CaaS.Infrastructure.Ado.Model;

namespace CaaS.Infrastructure.Repositories.Base; 

public interface IDomainReadModelConverter<in TData, TDomain> {
    IEnumerable<OrderParameter>? DefaultOrderParameters { get; }

    ValueTask<TDomain> ConvertToDomain(TData dataModel, CancellationToken cancellationToken);

    Task<IReadOnlyList<TDomain>> ConvertToDomain(IAsyncEnumerable<TData> dataModels, CancellationToken cancellationToken = default);
}