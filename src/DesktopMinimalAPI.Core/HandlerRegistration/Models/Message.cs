using System.Diagnostics.CodeAnalysis;

namespace DesktopMinimalAPI.Core.HandlerRegistration.Models;

public readonly record struct Message(string Value)
{
    public Message() : this(string.Empty) { }

    [SuppressMessage("Design", "CA2225: Operator overloads have named alternates",
        Justification = "This is a lighweight wrapper record struct, only intented to use" +
        "as a marker, and get its content implicitly.")]
    public static implicit operator Message(string value) => new(value);

    public static implicit operator string(Message wrappee) => wrappee.Value;

}
