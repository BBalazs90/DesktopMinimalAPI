using DesktopMinimalAPI.Core.RequestHandling.Models;
using FluentAssertions;
using LanguageExt.UnsafeValueAccess;
using System.Collections.Immutable;

using static DesktopMinimalAPI.Core.ParameterReading.ParameterReader;

namespace DesktopMinimalAPI.Core.Tests;
public class WhenParametersRead
{
    [Fact]
    public void ShouldReaturnRequestedParameterFromUrl()
    {
        var expected = "yaaaay";
        ImmutableArray<UrlParameterString> urlParams = [new UrlParameterString(expected)];
        var emptyBody = JsonBody.From("").ValueUnsafe();

        var parameter = GetParameter<string>(urlParams, emptyBody);

        _ = parameter.IsSome.Should().BeTrue();
        _ = parameter.ValueUnsafe().Should().Be(expected);
    }

    
}
