using System.Collections.Immutable;
using System.Runtime.Serialization;
using CaaS.Core.Base.Validation;

namespace CaaS.Core.Base.Exceptions; 

public class CaasPaymentInsufficientCreditException : CaasException {
    public IEnumerable<ValidationFailure> Errors { get; private set; } = ImmutableArray<ValidationFailure>.Empty;
    
    public CaasPaymentInsufficientCreditException() { }
    protected CaasPaymentInsufficientCreditException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public CaasPaymentInsufficientCreditException(string? message) : base(message) { }
    public CaasPaymentInsufficientCreditException(string? message, Exception? innerException) : base(message, innerException) { }

    public CaasPaymentInsufficientCreditException(IEnumerable<ValidationFailure> errors) {
        Errors = errors;
    }
    
    public CaasPaymentInsufficientCreditException(ValidationFailure error) {
        Errors = new ValidationFailure[]{error};
    }
    
    public CaasPaymentInsufficientCreditException(string message, IEnumerable<ValidationFailure> errors) : base(message) {
        Errors = errors;
    }
}