using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.ParameterReading;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using FluentAssertions;
using LanguageExt.UnsafeValueAccess;
using System.Text.Json;

namespace DesktopMinimalAPI.Core.Tests;
public class WhenBodyParametersRead
{
    [Fact]
    public void ShouldReaturnRequestedRarameterFromBody()
    {
        var expectedContent = new Testrecord("param1", "param2");
        var body = JsonBody.From(JsonSerializer.Serialize(expectedContent, Serialization.DefaultCamelCase)).ValueUnsafe();

        var parameter = ParameterReader.GetBodyParameter<Testrecord>(body);

        _ = parameter.IsSome.Should().BeTrue();
        _ = parameter.ValueUnsafe().Should().Be(expectedContent);
    }

    public record Testrecord(string P1, string P2);
}
