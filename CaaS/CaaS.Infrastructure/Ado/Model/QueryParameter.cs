namespace CaaS.Infrastructure.Ado.Model;

public record QueryParameter(string Name, object? Value, TypeCode? TypeCode = null) {
    public string ParameterName { get; init; } = Name;
};