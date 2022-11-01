namespace CaaS.Infrastructure.DataMapping; 

public interface IRecord {
    IRecordValues ByColumName();

    IRecordValues ByPropertyName();
}