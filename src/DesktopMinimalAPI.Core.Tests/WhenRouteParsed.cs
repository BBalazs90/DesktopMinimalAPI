using DesktopMinimalAPI.Core.RequestHandling.Models;
using FluentAssertions;
using LanguageExt.UnsafeValueAccess;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopMinimalAPI.Core.Tests;
public sealed class WhenRouteParsed
{
    [Theory]
    [InlineData("/validRoute")]
    [InlineData("/")]
    [InlineData("/this_is_valid_too")]
    [InlineData("/so_is+this")]
    public void ShouldContainPathAndEmptyParameterListIfNoParametersProvided(string maybeRoute)
    {
        var route = Route.From(maybeRoute);

        _ = route.IsRight.Should().BeTrue();
        _ = route.ValueUnsafe().Path.Value.Should().Be(maybeRoute);
        _ = route.ValueUnsafe().Parameters.Should().BeEmpty();
    }

    [Theory]
    [InlineData("invalidRoute")]
    [InlineData("invalidRoute?p1=1")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ShouldBeNoneIfInvalidRouteStringProvided(string? maybeRoute)
    {
        var route = Route.From(maybeRoute);

        _ = route.IsLeft.Should().BeTrue();
    }

    [Theory]
    [InlineData("/valid?param1=1", "/valid", "1")]
    [InlineData("/valid?param1=1&param2=2", "/valid", "1", "2")]
    [InlineData("/valid?param1=1&param2=2&param3=3", "/valid", "1", "2", "3")]
    [InlineData("/valid?param1=1.27", "/valid", "1.27")]
    [InlineData("/valid?param1=-1.27", "/valid", "-1.27")]
    [InlineData("/valid?param1=-2", "/valid", "-2")]
    [InlineData("/valid?param1=abcdfghi", "/valid", "abcdfghi")]
    public void ShouldContainPathAndParameterListIfParametersProvided(string maybeRoute, string expectedPath, params string[] parameters)
    {
        var route = Route.From(maybeRoute);

        _ = route.IsRight.Should().BeTrue();
        _ = route.ValueUnsafe().Path.Value.Should().Be(expectedPath);
        _ = route.ValueUnsafe().Parameters.Should().ContainInOrder(parameters.Select(ps => new UrlParameterString(ps)));
    }

    [Theory]
    [InlineData("/valid?param1=")]
    public void ShouldNotReturnRouteButIndicateFailureIfParametersNotProperlyProvided(string maybeRoute)
    {
        var route = Route.From(maybeRoute);

        _ = route.IsLeft.Should().BeTrue();
    }
}
