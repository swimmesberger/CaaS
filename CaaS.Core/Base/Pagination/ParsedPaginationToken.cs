namespace CaaS.Core.Base.Pagination;

public record ParsedPaginationToken(KeysetPaginationDirection Direction = KeysetPaginationDirection.Forward, SkipTokenValue? Reference = null, int? Limit = null) {
    public const int DefaultPageLimit = 20;
    public const int MaximumPageLimit = 100;
    
    public static readonly ParsedPaginationToken First = new ParsedPaginationToken();
    public static readonly ParsedPaginationToken Last = new ParsedPaginationToken(KeysetPaginationDirection.Backward);
};