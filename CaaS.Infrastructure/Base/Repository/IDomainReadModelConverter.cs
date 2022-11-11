using CaaS.Infrastructure.Base.Ado.Model;

namespace CaaS.Infrastructure.Base.Repository; 

public interface IDomainReadModelConverter<in TData, TDomain> {
    IEnumerable<OrderParameter>? DefaultOrderParameters { get; }

    ValueTask<TDomain> ConvertToDomain(TData dataModel, CancellationToken cancellationToken);

    Task<IReadOnlyList<TDomain>> ConvertToDomain(IAsyncEnumerable<TData> dataModels, CancellationToken cancellationToken = default);
}