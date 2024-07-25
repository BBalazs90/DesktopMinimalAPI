using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DesktopMinimalAPI.Models;

[JsonConverter(typeof(MethodsJsonConverter))]
public abstract class Methods
{
    public static readonly Get Get = new();
    public static readonly Post Post = new();
    public static readonly Invalid Invalid = new();


    public static explicit operator Methods(string method) => method.ToUpper() switch
    {
        "GET" => Get,
        "POST" => Post,
        _ => Invalid
    };

}

public sealed class Get : Methods
{
    internal Get() { }
    public override string ToString() => "GET";
}

public sealed class Post : Methods
{
    internal Post() { }
    public override string ToString() => "POST";
};

public sealed class Invalid : Methods
{
    internal Invalid() { }
    public override string ToString() => "INVALID";
};

public class MethodsJsonConverter : JsonConverter<Methods>
{
    public override Methods Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            return Methods.Invalid;
        }

        return (Methods)(reader.GetString() ?? string.Empty) ?? Methods.Invalid;

    }

    public override void Write(Utf8JsonWriter writer, Methods value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
