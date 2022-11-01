namespace CaaS.Infrastructure.DataMapping; 

public interface IPropertyMapping {
    IPropertyMapper ByColumName();

    IPropertyMapper ByPropertyName();
}