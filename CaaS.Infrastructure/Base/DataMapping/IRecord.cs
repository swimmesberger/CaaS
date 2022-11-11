namespace CaaS.Infrastructure.Base.DataMapping; 

public interface IRecord {
    IRecordValues ByColumName();

    IRecordValues ByPropertyName();
}