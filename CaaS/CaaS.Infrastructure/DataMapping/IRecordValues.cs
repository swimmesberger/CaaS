namespace CaaS.Infrastructure.DataMapping; 

public interface IRecordValues : IPropertyMapper {
    object? GetObject(string key);
}