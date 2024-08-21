using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using LanguageExt.UnsafeValueAccess;
using System.Collections.Immutable;
using System.Linq;

namespace DesktopMinimalAPI.Core;

internal static class RoutePipeline
{

    public static ImmutableArray<(string Name, string Value)> GetParameters(string path) =>
        path switch
        {
            _ when path.IndexOf('?') > 0 => path
                .Split('?')[1]
                .Split('&')
                .Select(x => { 
                    var nameValue = x.Split('='); 
                    return (nameValue[0], nameValue[1]); })
                .ToImmutableArray(),
            _ => ImmutableArray<(string, string)>.Empty
        };
}
