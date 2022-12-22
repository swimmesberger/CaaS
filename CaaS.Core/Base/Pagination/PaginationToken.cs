namespace CaaS.Core.Base.Pagination;

public record PaginationToken(KeysetPaginationDirection Direction = KeysetPaginationDirection.Forward, string? Reference = null, int? Limit = null);