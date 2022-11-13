namespace CaaS.Api.Base.Attributes; 

public class ProducesTotalCountHeaderAttribute : ProducesHeaderAttribute {
    public ProducesTotalCountHeaderAttribute() : base(HeaderConstants.TotalCount, typeof(long), HeaderConstants.TotalCountDescription) { }
}