// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace CaaS.Core.Base.Pagination;

public record PagedResult<T> : CountedResult<T> {
    public long TotalPages { get; init; }
    public ParsedPaginationToken FirstPage { get; init; } = ParsedPaginationToken.First;
    public ParsedPaginationToken? PreviousPage { get; init; } = ParsedPaginationToken.First;
    public ParsedPaginationToken? NextPage { get; init; } = ParsedPaginationToken.First;
    public ParsedPaginationToken LastPage { get; init; } = ParsedPaginationToken.Last;
}