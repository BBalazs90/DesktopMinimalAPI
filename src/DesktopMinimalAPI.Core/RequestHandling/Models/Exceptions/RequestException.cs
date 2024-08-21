using System;
using System.Diagnostics.CodeAnalysis;

namespace DesktopMinimalAPI.Core.RequestHandling.Models.Exceptions;

[SuppressMessage("Design", "CA1032:Implement standard exception constructors",
    Justification = "This exception is not meant to be instantiated alone, but should contain" +
    "an inner exception describing what went wrong during the request processing.")]
public class RequestException : Exception
{
    public Guid RequestId { get; } = Guid.Empty;

    private  RequestException(string message, Exception innerException) 
        : base(message, innerException) { }
    private RequestException(RequestId requestId, string message, Exception innerException) 
        : base(message, innerException) 
        => RequestId = requestId;

    public static RequestException From(Exception ex) 
        => new("Invalid request! See inner exception for details.", ex);
    public static RequestException From(RequestId requestId, Exception ex) 
        => new(requestId, "Invalid request! See inner exception for details.", ex);
}
