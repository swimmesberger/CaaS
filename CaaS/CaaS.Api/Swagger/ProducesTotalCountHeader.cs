namespace CaaS.Api.Swagger; 

public class ProducesTotalCountHeader : ProducesHeaderAttribute {
    public ProducesTotalCountHeader() : base(HeaderConstants.TotalCount, typeof(long), HeaderConstants.TotalCountDescription) { }
}