﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DesktopMinimalAPI.Models;

[JsonConverter(typeof(MethodsJsonConverter))]
public abstract record Methods
{
    public static explicit operator Methods(string method) => method.ToUpper() switch
    {
        "GET" => new Get(),
        "POST" => new Post(),
        _ => throw new ArgumentException()
    };

    internal sealed record Get : Methods { public override string ToString() => "GET";}
    internal sealed record Post : Methods { public override string ToString() => "POST"; };
}
public class MethodsJsonConverter : JsonConverter<Methods>
{
    public override Methods Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException();
        }

        string method = reader.GetString();

        return (Methods)method;
    }

    public override void Write(Utf8JsonWriter writer, Methods value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
