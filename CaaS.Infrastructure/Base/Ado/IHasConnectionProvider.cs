namespace CaaS.Infrastructure.Base.Ado; 

public interface IHasConnectionProvider {
    IConnectionProvider ConnectionProvider { get; }
}