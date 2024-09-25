using DesktopMinimalAPI.Core.RequestHandling.Models;
using FluentAssertions;
using LanguageExt.UnsafeValueAccess;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using static DesktopMinimalAPI.Core.ParameterReading.ParameterReader;
using static DesktopMinimalAPI.Core.Tests.WhenUrlParametersRead;

namespace DesktopMinimalAPI.Core.Tests;
public class WhenUrlParametersRead
{
    [Fact]
    public void ShouldReaturnRequestedParameterFromUrl()
    {
        const string expectedContent = "yaaaay";
        ImmutableArray<UrlParameterString> urlParams = [new UrlParameterString(expectedContent)];

        var parameter = GetUrlParameter<string>(urlParams);

        _ = parameter.IsSome.Should().BeTrue();
        _ = parameter.ValueUnsafe().Should().Be(expectedContent);
    }

    [Fact]
    public void ShouldReaturnNoneIfParameterNotProvidedInUrl()
    {
        ImmutableArray<UrlParameterString> urlParams = [];

        var parameter = GetUrlParameter<string>(urlParams);

        _ = parameter.IsNone.Should().BeTrue();
    }

    [Fact]
    public void ShouldReaturnNoneIfParameterNotConvertible()
    {
        ImmutableArray<UrlParameterString> urlParams = [new UrlParameterString("42")];

        var parameter = GetUrlParameter<NotConvertibleWrapper>(urlParams);

        _ = parameter.IsNone.Should().BeTrue();
    }

    [Fact]
    public void ShouldReaturnRequestedParameterIfConvertible()
    {
        ImmutableArray<UrlParameterString> urlParams = [new UrlParameterString("42")];

        var parameter = GetUrlParameter<JsonConvertibleWrapper>(urlParams);

        _ = parameter.IsSome.Should().BeTrue();
        _ = parameter.ValueUnsafe().Should().Be(new JsonConvertibleWrapper(42));
    }

    [Fact]
    public void ShouldReaturnRequested2ParametersFromUrl()
    {
        const string expectedContent1 = "yaaaay1";
        const string expectedContent2 = "yaaaay2";
        ImmutableArray<UrlParameterString> urlParams = [new UrlParameterString(expectedContent1), 
            new UrlParameterString(expectedContent2)];

        var parameters = GetUrlParameters<string, string>(urlParams);

        _ = parameters.IsSome.Should().BeTrue();
        var (p1, p2) = parameters.ValueUnsafe();
        _ = p1.Should().Be(expectedContent1);
        _ = p2.Should().Be(expectedContent2);
    }

    [Fact]
    public void ShouldReaturnRequested3ParametersFromUrl()
    {
        const string expectedContent1 = "yaaaay1";
        const string expectedContent2 = "yaaaay2";
        const string expectedContent3 = "yaaaay3";
        ImmutableArray<UrlParameterString> urlParams = [new UrlParameterString(expectedContent1),
            new UrlParameterString(expectedContent2),
            new UrlParameterString(expectedContent3)];

        var parameters = GetUrlParameters<string, string, string>(urlParams);

        _ = parameters.IsSome.Should().BeTrue();
        var (p1, p2, p3) = parameters.ValueUnsafe();
        _ = p1.Should().Be(expectedContent1);
        _ = p2.Should().Be(expectedContent2);
        _ = p3.Should().Be(expectedContent3);
    }

    [Fact]
    public void ShouldReaturnRequested4ParametersFromUrl()
    {
        const string expectedContent1 = "yaaaay1";
        const string expectedContent2 = "yaaaay2";
        const string expectedContent3 = "yaaaay3";
        const string expectedContent4 = "yaaaay4";
        ImmutableArray<UrlParameterString> urlParams = [new UrlParameterString(expectedContent1),
            new UrlParameterString(expectedContent2),
            new UrlParameterString(expectedContent3),
            new UrlParameterString(expectedContent4)];

        var parameters = GetUrlParameters<string, string, string, string>(urlParams);

        _ = parameters.IsSome.Should().BeTrue();
        var (p1, p2, p3, p4) = parameters.ValueUnsafe();
        _ = p1.Should().Be(expectedContent1);
        _ = p2.Should().Be(expectedContent2);
        _ = p3.Should().Be(expectedContent3);
        _ = p4.Should().Be(expectedContent4);
    }

    public readonly record struct NotConvertibleWrapper(int Value);
    [JsonConverter(typeof(JsonConvertibleWrapperConverter))]public readonly record struct JsonConvertibleWrapper(int Value);
}

public class JsonConvertibleWrapperConverter : JsonConverter<JsonConvertibleWrapper>
{
    public override JsonConvertibleWrapper Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.Number)
        {
            throw new JsonException($"Unexpected token type '{reader.TokenType}' when deserializing SwedId.");
        }

        var value = reader.GetInt32();
        return new JsonConvertibleWrapper(value);
    }

    public override void Write(Utf8JsonWriter writer, JsonConvertibleWrapper value, JsonSerializerOptions options) => 
        writer.WriteNumberValue(value.Value);
}
