namespace CaaS.Infrastructure.Base.Mapping; 

public interface IRecord {
    IRecordValues ByColumName();

    IRecordValues ByPropertyName();
}