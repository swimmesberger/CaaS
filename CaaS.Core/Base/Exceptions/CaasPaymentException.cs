using System.Runtime.Serialization;
using CaaS.Core.Base.Validation;

namespace CaaS.Core.Base.Exceptions; 

public class CaasPaymentException : CaasValidationException {
    protected CaasPaymentException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public CaasPaymentException(string? message = null, Exception? innerException = null) : base(message, innerException) { }
    public CaasPaymentException(ValidationFailure error) : base(error) { }
    public CaasPaymentException(string message, IEnumerable<ValidationFailure> errors) : base(message, errors) { }
    public CaasPaymentException(IEnumerable<ValidationFailure> errors, string? message = null, Exception? innerException = null) : base(errors, message, innerException) { }
}