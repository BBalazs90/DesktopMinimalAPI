using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Models;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;

namespace DesktopMinimalAPI.Core;
internal static class RequestTransformerPipeline
{
    public static TransformedWmRequest Transform(WmRequest original) => new(
            original.RequestId,
            original.Method,
            RoutePipeline
                .GetParameters(original.Path)
                .Select(nameValue => new RequestParameterIntermediate(nameValue.Name, nameValue.Value))
                .ToImmutableArray()
                .AddRange(GetParametersFromBody(original.Body)));

    private static ImmutableArray<RequestParameterIntermediate> GetParametersFromBody(string? body)
    {
        if (body is null)
        {
            return [];
        }

        try
        {
            var parameters = JsonSerializer.Deserialize<Dictionary<string, string>>(body, Serialization.DefaultCamelCase) ?? [];
            return parameters
                .Select(nameValue => new RequestParameterIntermediate(nameValue.Key, nameValue.Value))
                .ToImmutableArray();
        }
        catch (JsonException)
        {
            return [];
        }
    }
}
