namespace DesktopMinimalAPI.Core.Abstractions;

#pragma warning disable CA1040 // Avoid empty interfaces
// This will be extended later
public interface IRoute
#pragma warning restore CA1040 // Avoid empty interfaces
{
}

public record StringRoute(string Path) : IRoute
{
    public static StringRoute ToStringRoute(string path) => new(path);

    public static explicit operator StringRoute(string path) => ToStringRoute(path);
}
