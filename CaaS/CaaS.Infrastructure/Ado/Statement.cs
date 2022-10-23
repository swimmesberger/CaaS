namespace CaaS.Infrastructure.Ado; 

public record Statement(string Sql, IEnumerable<QueryParameter>? Parameters = null);