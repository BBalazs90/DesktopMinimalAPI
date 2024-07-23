using DesktopMinimalAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopMinimalAPI.Core.Tests;

public class WhenGetRequestReceived
{
    [Fact]
    public void ShouldReturnOkResponse()
    {
        // Arrange
        var request = new WmRequest("asd", new Methods.Get(), "/test");

        //var handler = new Func<WmRequest, WmResponse>(req =>
        //{
        //    return new WmResponse
        //    {
        //        StatusCode = 200,
        //        Body = "Hello, World!"
        //    };
        //});

        //var builder = new Builder()
        //    .MapGet(new StringRoute("test"), handler);

        //var broker = builder.BuildAsync().Result;

        //// Act
        //broker.OnWebMessageReceived(null, new CoreWebView2WebMessageReceivedEventArgs
        //{
        //    WebMessageAsJson = JsonSerializer.Serialize(request, Serialization.DefaultCamelCase)
        //});

        //// Assert
        //broker.CoreWebView.PostWebMessageAsString(Arg.Any<string>()).Received().PostWebMessageAsString(Arg.Any<string>());
    }
}
