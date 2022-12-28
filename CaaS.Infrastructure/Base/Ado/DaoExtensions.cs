using CaaS.Infrastructure.Base.Ado.Query.Parameters;
using CaaS.Infrastructure.Base.Ado.Query.Parameters.Where;
using CaaS.Infrastructure.Base.Model;

namespace CaaS.Infrastructure.Base.Ado; 

public static class DaoExtensions {
    public static async Task<T?> FindByIdAsync<T>(this IDao<T> dao, Guid id, CancellationToken cancellationToken = default) {
        return await dao.FindBy(new StatementParameters() {
            WhereParameters = new WhereParameters(nameof(IDataModelBase.Id), id)
        }, cancellationToken).FirstOrDefaultAsync(cancellationToken);
    }
    
    public static IAsyncEnumerable<T> FindByIdsAsync<T>(this IDao<T> dao, IEnumerable<Guid> ids, CancellationToken cancellationToken = default) {
        if (!ids.TryGetNonEnumeratedCount(out var count)) {
            var colSet = ids.ToHashSet();
            count = colSet.Count;
            ids = colSet;
        }
        if (count == 0) {
            return AsyncEnumerable.Empty<T>();
        }
        return dao.FindBy(StatementParameters.CreateWhere(nameof(IDataModelBase.Id), ids), cancellationToken);
    }
    
    public static Task<bool> AnyAsync<T>(this IDao<T> dao, StatementParameters? parameters = null, CancellationToken cancellationToken = default) {
        parameters ??= new StatementParameters() {
            SelectParameters = SelectParameters.Empty
        };
        parameters = parameters.WithLimit(1);
        return dao.FindScalarBy<object>(parameters, cancellationToken).AnyAsync(cancellationToken).AsTask();
    }
}