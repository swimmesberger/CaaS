namespace CaaS.Infrastructure.DataMapping.Base; 

public interface IPropertyMapping {
    IPropertyMapper ByColumName();

    IPropertyMapper ByPropertyName();
}