using CaaS.Core.Base;
using CaaS.Core.Base.Pagination;

namespace CaaS.Core.ProductAggregate; 

public interface IProductRepository : IRepository {
    Task<Product?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<PagedResult<Product>> FindByTextSearchAsync(string? text, PaginationToken? paginationToken = null, CancellationToken cancellationToken = default);
    Task<PagedResult<Product>> FindAllPagedAsync(PaginationToken? paginationToken = null, CancellationToken cancellationToken = default);
    Task<Product> AddAsync(Product entity, CancellationToken cancellationToken = default);
    Task<Product> UpdateAsync(Product oldEntity, Product newEntity, CancellationToken cancellationToken = default);
    Task<long> CountAsync(CancellationToken cancellationToken = default);
}