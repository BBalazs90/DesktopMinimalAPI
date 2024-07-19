namespace DesktopMinimalAPI.Core.Abstractions;

public interface IRoute
{
}

public record StringRoute(string Path) : IRoute
{
    public static explicit operator StringRoute(string path) => new(path);
}
