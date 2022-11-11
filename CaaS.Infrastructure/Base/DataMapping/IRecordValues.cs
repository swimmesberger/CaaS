namespace CaaS.Infrastructure.Base.DataMapping; 

public interface IRecordValues : IPropertyMapper {
    public const int DbTypeJson = 10_000;
    public const int DbTypeUndefined = 0;
    
    object? GetObject(string key);
    
    int? GetObjectType(string key);
}