namespace CaaS.Core.Base;

public record PaginationToken(KeysetPaginationDirection Direction = KeysetPaginationDirection.Forward, string? Reference = null);