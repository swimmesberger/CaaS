namespace CaaS.Infrastructure.Repositories.Base.Mapping; 

public interface IRecordValues : IPropertyMapper {
    DateTimeOffset? GetDateTimeOffset(string key);

    object? GetObject(string key);

    object? GetObject(string key, Type targetType);
    
    int? GetIn32(string key);

    Guid? GetGuid(string key);

    string? GetString(string key);
}

public interface IRecord {
    IRecordValues ByColumName();

    IRecordValues ByPropertyName();
}