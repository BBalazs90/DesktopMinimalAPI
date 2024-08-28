using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.Tests.Helpers;
using FluentAssertions;
using System.Net;
using System.Text.Json;
using DesktopMinimalAPI.Extensions;

namespace DesktopMinimalAPI.Core.Tests;
public sealed class WhenPostRequestReceived
{
    private readonly WebMessageBrokerBuilderForTest _builder;
    private const string _testPath = "/test";

    public WhenPostRequestReceived()
    {
        _builder = new WebMessageBrokerBuilderForTest();
    }

    [Fact]
    public async Task ShouldReturnOkResponseWithHandlerContentAndRequestGuid()
    {
        const string HandlerReturn = "Awesome, I work!";
        var handler = () => HandlerReturn;
        _ = _builder.MapPost(_testPath, handler);
        _ = await _builder!.BuildAsync();

        var guid = _builder.MockCoreWebView2.SimulatePost(_testPath);

        var response = await _builder.MockCoreWebView2.ReadLastResponseAsync();
        _ = response.Status.Should().Be(HttpStatusCode.OK);
        _ = response.RequestId.Should().Be(guid);
        _ = JsonSerializer.Deserialize<string>(response.Data, Serialization.DefaultCamelCase).Should().Be(HandlerReturn);
    }
}
