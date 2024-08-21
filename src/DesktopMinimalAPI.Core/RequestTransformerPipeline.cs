using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using System.Collections.Immutable;
using System.Linq;

namespace DesktopMinimalAPI.Core;
internal static class RequestTransformerPipeline
{
    public static TransformedWmRequest Transform(WmRequest original) => new(
            original.Id,
            original.Method,
            RoutePipeline
                .GetParameters(original.Route.Path)
                .Select(nameValue => new RequestParameterIntermediate(nameValue.Name, nameValue.Value))
                //.Append(new RequestParameterIntermediate("body", original.Body))
                .ToImmutableArray());
}
