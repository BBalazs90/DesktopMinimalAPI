using LanguageExt;
using System;

namespace DesktopMinimalAPI.Core.RequestHandling.Models;
public readonly record struct JsonBody
{
    private const char _startObject = '{';
    private const char _endObject = '}';

    public static Option<JsonBody> From(string? maybeJson) =>
        maybeJson is not null && IsValidJson(maybeJson)
        ? new JsonBody(maybeJson)
        : Option<JsonBody>.None;

    public JsonBody() : this(string.Empty) { } // throws

    private JsonBody(string? value) => Value = value switch
    {
        string notNullValue when IsValidJson(notNullValue) => notNullValue,
        { } => throw new ArgumentException($"The provided argument is not a valid JSON string: {value}"),
        _ => throw new ArgumentNullException(nameof(value)),
    };

    private static bool IsValidJson(string maybeJson) => 
        maybeJson.StartsWith(_startObject) && maybeJson.EndsWith(_endObject);

    public string Value { get; }
}
