using System.Runtime.Serialization;

namespace CaaS.Core.Base.Exceptions; 

public class CaasUpdateConcurrencyDbException : CaasDbException {
    public CaasUpdateConcurrencyDbException() { }

    protected CaasUpdateConcurrencyDbException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    public CaasUpdateConcurrencyDbException(string? message) : base(message) { }

    public CaasUpdateConcurrencyDbException(string? message, Exception? innerException) : base(message, innerException) { }
}