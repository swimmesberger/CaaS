namespace CaaS.Generator.Common {
    public interface IPropertyMapping {
        IPropertyMapper ByColumName();

        IPropertyMapper ByPropertyName();
    }
}