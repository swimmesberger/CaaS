namespace CaaS.Infrastructure.Base.Ado.Model;

public record QueryParameter(string Name) {
    
    public string ParameterName { get; init; } = Name;

    public TypedValue TypedValue { get; init; }
    
    public Comparators Comparator { get; init; }

    public object? Value {
        get => TypedValue.Value;
        init => TypedValue = new TypedValue() { Value = value };
    }

    public static QueryParameter From(string name, object? value, string? parameterName = null, Comparators comparator = Comparators.Equal) {
        return FromTyped(name, new TypedValue { Value = value }, parameterName, comparator);
    }
    
    public static QueryParameter FromTyped(string name, TypedValue value = default, string? parameterName = null, Comparators comparator = Comparators.Equal) {
        parameterName ??= name;
        
        return new QueryParameter(name) {
            TypedValue = value, 
            ParameterName = parameterName,
            Comparator = comparator
        };
    }
};