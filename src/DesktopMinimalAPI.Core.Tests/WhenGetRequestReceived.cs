using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.Tests.Helpers;
using DesktopMinimalAPI.Extensions;
using System.Text.Json;
using FluentAssertions;

namespace DesktopMinimalAPI.Core.Tests;

public class WhenGetRequestReceived
{
    private readonly WebMessageBrokerBuilderForTest _builder;
    private const string _testPath = "/test";

    public WhenGetRequestReceived()
    {
        _builder = new WebMessageBrokerBuilderForTest();
    }

    [Fact]
    public async Task ShouldReturnOkResponse()
    {
        // Arrange
        const string HandlerReturn = "Awesome, I work!";
        var handler = () => HandlerReturn;
        _builder.MapGet(_testPath, handler);
        var broker = await _builder!.BuildAsync();

        //// Act
        var guid = _builder.MockCoreWebView2.SimulateGet(_testPath);

        //// Assert
        var response = _builder.MockCoreWebView2.ReadLastResponse();
        response.Status.Should().Be(200);
        response.RequestId.Should().Be(guid);
        JsonSerializer.Deserialize<string>(response.Data, Serialization.DefaultCamelCase).Should().Be(HandlerReturn);
    }
}
