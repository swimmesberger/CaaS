namespace CaaS.Core.Base.Pagination;

public record ParsedPaginationToken(KeysetPaginationDirection Direction = KeysetPaginationDirection.Forward, SkipTokenValue? Reference = null) {
    public const long DefaultPageSize = 20;
    
    public static readonly ParsedPaginationToken First = new ParsedPaginationToken();
    public static readonly ParsedPaginationToken Last = new ParsedPaginationToken(KeysetPaginationDirection.Backward);
};