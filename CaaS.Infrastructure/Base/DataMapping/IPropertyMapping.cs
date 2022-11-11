namespace CaaS.Infrastructure.Base.DataMapping; 

public interface IPropertyMapping {
    IPropertyMapper ByColumName();

    IPropertyMapper ByPropertyName();
}