using System.Runtime.Serialization;

namespace CaaS.Core.Exceptions; 

public class CaasInsertDbException : CaasDbException {
    public CaasInsertDbException() { }
    protected CaasInsertDbException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public CaasInsertDbException(string? message) : base(message) { }
    public CaasInsertDbException(string? message, Exception? innerException) : base(message, innerException) { }
}