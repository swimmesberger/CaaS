using System.Runtime.Serialization;

namespace CaaS.Core.Repositories.Base; 

public class DbUpdateConcurrencyException : Exception {
    public DbUpdateConcurrencyException() { }

    protected DbUpdateConcurrencyException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    public DbUpdateConcurrencyException(string? message) : base(message) { }

    public DbUpdateConcurrencyException(string? message, Exception? innerException) : base(message, innerException) { }
}