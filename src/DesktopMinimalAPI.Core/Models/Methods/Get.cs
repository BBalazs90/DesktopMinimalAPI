using System.Diagnostics.CodeAnalysis;

namespace DesktopMinimalAPI.Core.Models.Methods;

[SuppressMessage("Usage", "CA1716: Identifiers should not match keywords",
    Justification = " This is a standard keyword for a RESTful API, must stick to the name.")]
public sealed class Get : Method
{
    internal Get() { }
    public override string ToString() => "GET";
}
