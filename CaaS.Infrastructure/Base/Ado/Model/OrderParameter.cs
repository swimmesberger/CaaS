namespace CaaS.Infrastructure.Base.Ado.Model;

public record OrderParameter(string Name, OrderType OrderType = OrderType.Asc) {
    public static OrderParameter[] From(OrderParameter orderParameter) => new []{ orderParameter };
    
    public static OrderParameter[] From(string name) => new []{ new OrderParameter(name) };
};

public enum OrderType {
    Asc,
    Desc
}