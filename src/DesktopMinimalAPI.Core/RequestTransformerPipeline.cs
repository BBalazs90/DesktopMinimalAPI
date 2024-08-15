using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Models;
using System.Collections.Immutable;
using System.Linq;

namespace DesktopMinimalAPI.Core;
internal static class RequestTransformerPipeline
{
    public static TransformedWmRequest Transform(WmRequestType original) => new(
            original.RequestId,
            original.Method,
            RoutePipeline
                .GetParameters(original.Path)
                .Select(nameValue => new RequestParameterIntermediate(nameValue.Name, nameValue.Value))
                .Append(new RequestParameterIntermediate("body", original.Body))
                .ToImmutableArray());
}
