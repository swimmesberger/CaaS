namespace CaaS.Infrastructure.Base.DataMapping; 

public interface IPropertyMapper {
    IEnumerable<string> Keys { get; }

    string MapName(string key);
}