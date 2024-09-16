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

        var parameter = GetParameter<FromUrl<string>, string>(urlParams, emptyBody, 0);

        _ = parameter.IsSome.Should().BeTrue();
        _ = parameter.ValueUnsafe().Should().Be(expectedParam);
    }

    [Fact]
    public void ShouldReaturnRequestedParametersFromUrl()
    {
        const string expectedContent1 = "yaaaay1";
        const string expectedContent2 = "yaaaay2";
        var expectedParam1 = new FromUrl<string>(expectedContent1);
        var expectedParam2 = new FromUrl<string>(expectedContent2);
        ImmutableArray<UrlParameterString> urlParams = [new UrlParameterString(expectedContent1), new UrlParameterString(expectedContent2)];
        var emptyBody = JsonBody.From("{}").ValueUnsafe();

        var parameter1 = GetParameter<FromUrl<string>, string>(urlParams, emptyBody, 0);
        var parameter2 = GetParameter<FromUrl<string>, string>(urlParams, emptyBody, 1);

        _ = parameter1.IsSome.Should().BeTrue();
        _ = parameter1.ValueUnsafe().Should().Be(expectedParam1);
        _ = parameter2.IsSome.Should().BeTrue();
        _ = parameter2.ValueUnsafe().Should().Be(expectedParam2);
    }

    [Fact]
    public void ShouldReaturnRequestedParameterFromBody()
    {
        var expectedContent = new TestRecord("param1", "param2");
        var expectedParam = new FromBody<TestRecord>(expectedContent);
        ImmutableArray<UrlParameterString> urlParams = [];
        var body = JsonBody.From(JsonSerializer.Serialize(expectedContent, Serialization.DefaultCamelCase)).ValueUnsafe();

        var parameter = GetParameter<FromBody<TestRecord>,TestRecord>(urlParams, body, 0);

        _ = parameter.IsSome.Should().BeTrue();
        _ = parameter.ValueUnsafe().Should().Be(expectedParam);
    }

    public record TestRecord(string P1, string P2);
}
