using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.Tests.Helpers;
using DesktopMinimalAPI.Models;
using DesktopMinimalAPI.Extensions;
using System.Text.Json;

namespace DesktopMinimalAPI.Core.Tests;

public class WhenGetRequestReceived
{
    [StaFact]
    public async Task ShouldReturnOkResponse()
    {
        // Arrange
        var handler = () => true;
        var builder = new WebMessageBrokerBuilderForTest()
            .MapGet("/test", handler) as WebMessageBrokerBuilderForTest;
        var broker = await builder!.BuildAsync();

        //// Act
        builder.MockCoreWebView2.RaiseWebMessageReceived(JsonSerializer.Serialize(new WmRequest("asd", (Methods)"GET", "/test"), Serialization.DefaultCamelCase));

        //// Assert
        Assert.True(!string.IsNullOrWhiteSpace(builder.MockCoreWebView2.LastPostedWebMessageAsString));
    }
}

