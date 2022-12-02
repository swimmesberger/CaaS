using CaaS.Core.Base;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Ado.Model.Where;
using CaaS.Infrastructure.Base.Mapping;

namespace CaaS.Infrastructure.Base.Repository; 

public static class PaginationExtensions {
    
    public static StatementParameters WithPagination(this StatementParameters statementParameters, long? pageSize = null, 
        Action<OrderParameterBuilder>? orderBuilderOptions = null, ParsedPaginationToken? paginationToken = null) {
        pageSize ??= ParsedPaginationToken.DefaultPageSize;
        paginationToken ??= ParsedPaginationToken.First;
        IReadOnlyList<OrderParameter> columns;
        if (orderBuilderOptions != null) {
            var builder = new OrderParameterBuilder(statementParameters.OrderBy);
            orderBuilderOptions(builder);
            columns = builder.Columns;
        } else {
            columns = statementParameters.OrderBy.ToList();
        }
        if (!columns.Any()) {
            throw new InvalidOperationException("There should be at least one configured column in the keyset.");
        }
        var where = statementParameters.WhereParameters;
        if (paginationToken.Reference != null) {
            var parameters = paginationToken.Reference.PropertyValues
                .Select(entry => QueryParameter.From(entry.Key, entry.Value)).ToList();
            var greaterThan = paginationToken.Direction switch {
                KeysetPaginationDirection.Forward => true,
                KeysetPaginationDirection.Backward => false,
                _ => throw new NotImplementedException(),
            };
            where = where.Add(new RowValueWhere(parameters, greaterThan ? WhereComparator.Greater : WhereComparator.Less));
        }
        var orderBy = columns.Select(c => {
            var isDescending = c.OrderType == OrderType.Desc;
            isDescending = paginationToken.Direction == KeysetPaginationDirection.Backward ? !isDescending : isDescending;
            return c with { OrderType = isDescending ? OrderType.Desc : OrderType.Asc };
        }).ToList();
        return statementParameters with {
            WhereParameters = where,
            OrderBy = orderBy,
            Limit = pageSize
        };
    }
    
    public static async Task<PagedResult<TR>> MapAsync<T, TR>(this PagedResult<T> result, Func<IEnumerable<T>, Task<IReadOnlyList<TR>>> selector) => new() {
        TotalCount = result.TotalCount,
        TotalPages = result.TotalPages,
        FirstPage = result.FirstPage,
        PreviousPage = result.PreviousPage,
        NextPage = result.NextPage,
        LastPage = result.LastPage,
        Items = await selector.Invoke(result)
    };
    
    public static async Task<PagedResult<T>> FindByPagedAsync<T>(this IDao<T> dao, StatementParameters parameters, long? pageSize = null, 
        PaginationToken? paginationToken = null, CancellationToken cancellationToken = default) {
        pageSize ??= ParsedPaginationToken.DefaultPageSize;
        var metadataProvider = dao.CreateMetadataProvider();
        var parsedPaginationToken = paginationToken == null ? null : 
            new ParsedPaginationToken(paginationToken.Direction, paginationToken.Reference == null ? null : 
                SkipTokenUtil.Parse(paginationToken.Reference, metadataProvider));
        var totalCount = await dao.CountAsync(parameters, cancellationToken: cancellationToken);
        var totalPages = (long)Math.Ceiling(totalCount / (double)pageSize);
        var paginationParameters = parameters.WithPagination(pageSize, null, parsedPaginationToken);
        var parameterNames = parameters.OrderBy.Select(o => o.Name).ToList();
        var data = await dao.FindBy(paginationParameters, cancellationToken).ToListAsync(cancellationToken);
        return new PagedResult<T>() {
            Items = data,
            PreviousPage = dao.CreatePaginationToken(data.FirstOrDefault(), parameterNames, KeysetPaginationDirection.Backward),
            NextPage = dao.CreatePaginationToken(data.LastOrDefault(), parameterNames, KeysetPaginationDirection.Forward),
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }

    private static IRecordMetadataProvider CreateMetadataProvider<T>(this IDao<T> dao) {
        IRecordMetadataProvider metadataProvider;
        if (dao is IHasMetadataProvider hasMetadataProvider) {
            metadataProvider = hasMetadataProvider.MetadataProvider;
        } else {
            throw new ArgumentException("Unsupported dao type");
        }
        return metadataProvider;
    }

    private static ParsedPaginationToken? CreatePaginationToken<T>(this IDao<T> dao, T? model, IEnumerable<string> properties, 
        KeysetPaginationDirection direction) {
        var token = model == null ? null : new SkipTokenValue() { PropertyValues = dao.ReadPropertiesFromModel(model, properties) };
        return token == null ? null : new ParsedPaginationToken(direction, token);
    }
}