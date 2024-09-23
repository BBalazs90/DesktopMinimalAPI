using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using FluentAssertions;
using LanguageExt.UnsafeValueAccess;
using System.Collections.Immutable;
using static DesktopMinimalAPI.Core.ParameterReading.ParameterReader;

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
    public void ShouldReaturnRequestedParametersFromUrl()
    {
        const string expectedContent1 = "yaaaay1";
        const string expectedContent2 = "yaaaay2";
        ImmutableArray<UrlParameterString> urlParams = [new UrlParameterString(expectedContent1), new UrlParameterString(expectedContent2)];

        var parameters = GetUrlParameters<string, string>(urlParams);

        _ = parameters.IsSome.Should().BeTrue();
        var (p1, p2) = parameters.ValueUnsafe();
        _ = p1.Should().Be(expectedContent1);
        _ = p2.Should().Be(expectedContent2);
    }

    [Fact]
    public void ShouldReaturnRequestedParameterFromBody()
    {
        //var expectedContent = new TestRecord("param1", "param2");
        //var expectedParam = new FromBody<TestRecord>(expectedContent);
        //ImmutableArray<UrlParameterString> urlParams = [];
        //var body = JsonBody.From(JsonSerializer.Serialize(expectedContent, Serialization.DefaultCamelCase)).ValueUnsafe();

        //var parameter = GetParameter<FromBody<TestRecord>,TestRecord>(urlParams, body, 0);

        //_ = parameter.IsSome.Should().BeTrue();
        //_ = parameter.ValueUnsafe().Should().Be(expectedParam);
    }

    public record TestRecord(string P1, string P2);
}
