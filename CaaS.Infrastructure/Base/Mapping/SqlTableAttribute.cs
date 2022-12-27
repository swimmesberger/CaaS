namespace CaaS.Infrastructure.Base.Mapping; 

public class SqlTableAttribute : Attribute {
    public string Value { get; }
    
    public SqlTableAttribute(string value) {
        Value = value;
    }
}