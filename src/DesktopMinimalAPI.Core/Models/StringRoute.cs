using DesktopMinimalAPI.Core.Abstractions;

namespace DesktopMinimalAPI.Core.Models;

public record StringRoute(string Path) : IRoute
{
    public static StringRoute ToStringRoute(string path) => new(path);

    public static explicit operator StringRoute(string path) => ToStringRoute(path);
}
