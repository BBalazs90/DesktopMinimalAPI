﻿using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Models;
using System.Collections.Immutable;
using System.Linq;

namespace DesktopMinimalAPI.Core;
internal static class RequestTransformerPipeline
{
    public static TransformedWmRequest Transform(WmRequest original)
    {
        return new TransformedWmRequest(
            original.RequestId,
            original.Method,
            RoutePipeline
                .GetParameters(original.Path)
                .Select(nameValue => new RequestParameterIntermediate(nameValue.Name, nameValue.Value))
                .ToImmutableArray());
    }
}
