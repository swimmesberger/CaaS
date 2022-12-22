using CaaS.Infrastructure.Base.Ado.Query;

namespace CaaS.Infrastructure.Base.Mapping; 

public class SqlIgnoreAttribute : Attribute {
    public StatementType[] Types { get; }
    
    public SqlIgnoreAttribute(params StatementType[] types) {
        Types = types;
    }
}