using System.Collections.Immutable;
using System.Runtime.Serialization;
using CaaS.Core.Base.Validation;

namespace CaaS.Core.Base.Exceptions; 

public class CaasPaymentException : CaasException {
    public IEnumerable<ValidationFailure> Errors { get; private set; } = ImmutableArray<ValidationFailure>.Empty;
    
    public CaasPaymentException() { }
    protected CaasPaymentException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public CaasPaymentException(string? message) : base(message) { }
    public CaasPaymentException(string? message, Exception? innerException) : base(message, innerException) { }

    public CaasPaymentException(IEnumerable<ValidationFailure> errors) {
        Errors = errors;
    }
    
    public CaasPaymentException(ValidationFailure error) {
        Errors = new ValidationFailure[]{error};
    }
    
    public CaasPaymentException(string message, IEnumerable<ValidationFailure> errors) : base(message) {
        Errors = errors;
    }
}