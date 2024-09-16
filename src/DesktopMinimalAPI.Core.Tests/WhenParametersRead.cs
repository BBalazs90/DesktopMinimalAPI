using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using FluentAssertions;
using LanguageExt.UnsafeValueAccess;
using System.Collections.Immutable;
using System.Text.Json;
using static DesktopMinimalAPI.Core.ParameterReading.ParameterReader;

namespace DesktopMinimalAPI.Core.Tests;
public class WhenParametersRead
{
    [Fact]
    public void ShouldReaturnRequestedParameterFromUrl()
    {
        const string expectedContent = "yaaaay";
        var expectedParam = new FromUrl<string>(expectedContent);
        ImmutableArray<UrlParameterString> urlParams = [new UrlParameterString(expectedContent)];
        var emptyBody = JsonBody.From("{}").ValueUnsafe();

        var parameter = GetParameter<FromUrl<string>, string>(urlParams, emptyBody);

        _ = parameter.IsSome.Should().BeTrue();
        _ = parameter.ValueUnsafe().Should().Be(expectedParam);
    }

    [Fact]
    public void ShouldReaturnRequestedParameterFromBody()
    {
        var expectedContent = new TestRecord("param1", "param2");
        var expectedParam = new FromBody<TestRecord>(expectedContent);
        ImmutableArray<UrlParameterString> urlParams = [];
        var body = JsonBody.From(JsonSerializer.Serialize(expectedContent, Serialization.DefaultCamelCase)).ValueUnsafe();

        var parameter = GetParameter<FromBody<TestRecord>,TestRecord>(urlParams, body);

        _ = parameter.IsSome.Should().BeTrue();
        _ = parameter.ValueUnsafe().Should().Be(expectedParam);
    }

    public record TestRecord(string P1, string P2);
}
