using System.Runtime.Serialization;

namespace CaaS.Core.Base.Exceptions; 

public class CaasConstraintViolationDbException : CaasDbException {
    public string? TableName { get; init; }
    public string? ConstraintName { get; init; }
    
    public CaasConstraintViolationDbException() { }
    protected CaasConstraintViolationDbException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public CaasConstraintViolationDbException(string? message) : base(message) { }
    public CaasConstraintViolationDbException(string? message, Exception? innerException) : base(message, innerException) { }
}