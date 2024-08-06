using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DesktopMinimalAPI.Core.Models.Methods;

[JsonConverter(typeof(MethodsJsonConverter))]
public abstract class Method
{
    public static readonly Get Get = new();
    public static readonly Post Post = new();

    public static Method ToMethod(string? method) => (method?.ToUpper(CultureInfo.CurrentCulture)
        ?? throw new ArgumentNullException(nameof(method))) switch
    {
        "GET" => Get,
        "POST" => Post,
        _ => throw new ArgumentOutOfRangeException(nameof(method), $"Expected a valid REST verb, got '{method}'"),
    };

    public static explicit operator Method(string? method) => ToMethod(method);
}

public class MethodsJsonConverter : JsonConverter<Method>
{
    public override Method Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType == JsonTokenType.String
        ? (Method)reader.GetString()
        : throw new ArgumentException($"Expected token: {JsonTokenType.String}, got {reader.TokenType}");

    [SuppressMessage("Usage", "CA1062: Validate arguments of public methods",
        Justification = "This method is only used by the JSON serializer framework, it guarantees the non-null writer.")]
    public override void Write(Utf8JsonWriter writer, Method value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value?.ToString() ?? throw new ArgumentNullException(nameof(value)));
}
