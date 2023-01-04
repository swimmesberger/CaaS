using System.Collections.Immutable;
using System.Runtime.Serialization;
using CaaS.Core.Base.Validation;
namespace CaaS.Core.Base.Exceptions; 

public class CaasValidationException : CaasException {
    public IEnumerable<ValidationFailure> Errors { get; private set; } = ImmutableArray<ValidationFailure>.Empty;
    public string? Type { get; init; }
    public string? Title { get; init; }
    public string? Detail { get; init; }
    

    protected CaasValidationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public CaasValidationException(string? message = null, Exception? innerException = null) : 
        this(Enumerable.Empty<ValidationFailure>(), message, innerException) {
    }
    
    public CaasValidationException(ValidationFailure error) : this(new[]{error}) { }
    
    public CaasValidationException(string message, IEnumerable<ValidationFailure> errors) : this(errors, message) {
    }

    public CaasValidationException(IEnumerable<ValidationFailure> errors, string? message = null, Exception? innerException = null) :
        base(message, innerException) {
        Errors = errors;
    }
}