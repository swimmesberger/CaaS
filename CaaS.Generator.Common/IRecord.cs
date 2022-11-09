namespace CaaS.Generator.Common {
    public interface IRecord {
        IRecordValues ByColumName();

        IRecordValues ByPropertyName();
    }
}