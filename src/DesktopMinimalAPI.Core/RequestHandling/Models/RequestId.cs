using LanguageExt;
using System;

namespace DesktopMinimalAPI.Core.RequestHandling.Models;

public readonly record struct RequestId
{
    public static Option<RequestId> From(string? maybeRequestId) =>
        Guid.TryParse(maybeRequestId, out var parsedGuid) && parsedGuid != Guid.Empty
            ? new RequestId(parsedGuid)
            : Option<RequestId>.None;

    public RequestId() : this(Guid.Empty) { } // Throws

    private RequestId(Guid value) => Value = value != Guid.Empty
        ? value
        : throw new ArgumentException("Value cannot be Guid.Empty.", nameof(value));

    public Guid Value { get; }

    public Guid ToGuid() => Value;
    public static implicit operator Guid(RequestId requestId) => requestId.ToGuid();
}
