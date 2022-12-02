using CaaS.Infrastructure.Base.Ado.Model.Where;

namespace CaaS.Infrastructure.Base.Ado.Model;

public record QueryParameter(string Name) {
    public string ParameterName { get; init; } = Name;

    public TypedValue TypedValue { get; init; }
    
    public WhereComparator Comparator { get; init; }

    public object? Value {
        get => TypedValue.Value;
        init => TypedValue = new TypedValue() { Value = value };
    }

    public QueryParameter(string name, WhereComparator comparator, object? value) : this(name) {
        Value = value;
        Comparator = comparator;
    }
    
    public QueryParameter(string name, object? value) : this(name, WhereComparator.Equal, value) { }

    public static QueryParameter From(string name, object? value, string? parameterName = null, WhereComparator comparator = WhereComparator.Equal) {
        return FromTyped(name, new TypedValue { Value = value }, parameterName, comparator);
    }
    
    public static QueryParameter FromTyped(string name, TypedValue value = default, string? parameterName = null, WhereComparator comparator = WhereComparator.Equal) {
        parameterName ??= name;
        
        return new QueryParameter(name) {
            TypedValue = value, 
            ParameterName = parameterName,
            Comparator = comparator
        };
    }
}