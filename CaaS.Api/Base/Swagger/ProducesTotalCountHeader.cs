namespace CaaS.Api.Base.Swagger; 

public class ProducesTotalCountHeader : ProducesHeaderAttribute {
    public ProducesTotalCountHeader() : base(HeaderConstants.TotalCount, typeof(long), HeaderConstants.TotalCountDescription) { }
}