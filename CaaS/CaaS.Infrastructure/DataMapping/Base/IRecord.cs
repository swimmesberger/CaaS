namespace CaaS.Infrastructure.DataMapping.Base; 

public interface IRecord {
    IRecordValues ByColumName();

    IRecordValues ByPropertyName();
}