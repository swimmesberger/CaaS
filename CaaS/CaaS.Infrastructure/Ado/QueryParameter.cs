namespace CaaS.Infrastructure.Ado;

public record QueryParameter(string Name, object? Value, TypeCode? TypeCode = null) {
    public string ParameterName { get; init; } = Name;
};