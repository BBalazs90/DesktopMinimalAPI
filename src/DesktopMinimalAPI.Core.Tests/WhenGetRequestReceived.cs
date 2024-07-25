using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.Tests.Helpers;
using DesktopMinimalAPI.Extensions;
using System.Text.Json;
using FluentAssertions;
using System.Net;

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
    public async Task ShouldReturnOkResponseWithHandlerContentAndRequestGuid()
    {
        const string HandlerReturn = "Awesome, I work!";
        var handler = () => HandlerReturn;
        _builder.MapGet(_testPath, handler);
        var broker = await _builder!.BuildAsync();

        var guid = _builder.MockCoreWebView2.SimulateGet(_testPath);

        var response = _builder.MockCoreWebView2.ReadLastResponse();
        response.Status.Should().Be(HttpStatusCode.OK);
        response.RequestId.Should().Be(guid);
        JsonSerializer.Deserialize<string>(response.Data, Serialization.DefaultCamelCase).Should().Be(HandlerReturn);
    }

    [Fact]
    public async Task ShouldReturnServerErrorResponseAndTheExceptionIfHandlerFailed()
    {
        const string errorMessage = "Oh, noooo!";
        Func<int> handler = () => throw new Exception(errorMessage); // Explicit type because from exception cannot be infered.
        _builder.MapGet(_testPath, handler);
        var broker = await _builder!.BuildAsync();

        //// Act
        var guid = _builder.MockCoreWebView2.SimulateGet(_testPath);

        //// Assert
        var response = _builder.MockCoreWebView2.ReadLastResponse();
        response.Status.Should().Be(HttpStatusCode.InternalServerError);
        response.RequestId.Should().Be(guid);
        JsonSerializer.Deserialize<string>(response.Data, Serialization.DefaultCamelCase).Should().Be(errorMessage);
    }
}
