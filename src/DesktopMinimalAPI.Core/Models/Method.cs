using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DesktopMinimalAPI.Models;

[JsonConverter(typeof(MethodsJsonConverter))]
public abstract class Method
{
    public static readonly Get Get = new();
    public static readonly Post Post = new();
    public static readonly Invalid Invalid = new();

    public static Method ToMethod(string method) => (method?.ToUpper(CultureInfo.CurrentCulture) 
        ?? throw new ArgumentNullException(nameof(method))) switch
    {
        "GET" => Get,
        "POST" => Post,
        _ => Invalid
    };

    public static explicit operator Method(string method) => ToMethod(method);

}

public sealed class Get : Method
{
    internal Get() { }
    public override string ToString() => "GET";
}

public sealed class Post : Method
{
    internal Post() { }
    public override string ToString() => "POST";
};

public sealed class Invalid : Method
{
    internal Invalid() { }
    public override string ToString() => "INVALID";
};

public class MethodsJsonConverter : JsonConverter<Method>
{
    public override Method Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            return Method.Invalid;
        }

        return (Method)(reader.GetString() ?? string.Empty) ?? Method.Invalid;

    }

    public override void Write(Utf8JsonWriter writer, Method value, JsonSerializerOptions options) => 
        writer?.WriteStringValue(value?.ToString() ?? throw new ArgumentNullException(nameof(value)));
}
