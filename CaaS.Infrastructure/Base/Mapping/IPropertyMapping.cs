namespace CaaS.Infrastructure.Base.Mapping; 

public interface IPropertyMapping {
    IPropertyMapper ByColumName();

    IPropertyMapper ByPropertyName();
}