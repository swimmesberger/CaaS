namespace CaaS.Infrastructure.Ado; 

public record QueryParameter(string Name, object? Value, TypeCode? TypeCode = null);