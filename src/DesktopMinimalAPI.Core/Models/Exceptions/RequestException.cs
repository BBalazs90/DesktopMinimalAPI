using System;
using System.Diagnostics.CodeAnalysis;

namespace DesktopMinimalAPI.Core.Models.Exceptions;

[SuppressMessage("Design", "CA1032:Implement standard exception constructors", 
    Justification = "This exception is not meant to be instantiated alone, but should contain" +
    "an inner exception describing what went wrong during the request processing.")]
public class RequestException : Exception
{
    private RequestException(string message, Exception innerException) : base(message, innerException) { }

    public static RequestException From(Exception ex) => new("Invalid request! See inner exception for details.", ex);
}
