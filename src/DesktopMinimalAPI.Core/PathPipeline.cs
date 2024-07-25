using DesktopMinimalAPI.Core.Abstractions;

namespace DesktopMinimalAPI.Core;

internal static class PathPipeline
{
    public static StringRoute GetRoot(string path) =>
        path switch
        {
            _ when path.IndexOf('?') > 0 => new StringRoute(path.Split('?')[0]),
            _ => new StringRoute(path)
        };
}
