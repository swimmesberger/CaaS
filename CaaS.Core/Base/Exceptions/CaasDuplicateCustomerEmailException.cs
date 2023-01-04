using System.Runtime.Serialization;

namespace CaaS.Core.Base.Exceptions; 

public class CaasDuplicateCustomerEmailException : CaasValidationException {
    public CaasDuplicateCustomerEmailException() { }
    protected CaasDuplicateCustomerEmailException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public CaasDuplicateCustomerEmailException(string? message) : base(message) { }
    public CaasDuplicateCustomerEmailException(string? message, Exception? innerException) : base(message, innerException) { }
}