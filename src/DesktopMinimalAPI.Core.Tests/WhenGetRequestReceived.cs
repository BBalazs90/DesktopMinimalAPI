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

        var guid = _builder.MockCoreWebView2.SimulateGet(_testPath);

        var response = _builder.MockCoreWebView2.ReadLastResponse();
        response.Status.Should().Be(HttpStatusCode.InternalServerError);
        response.RequestId.Should().Be(guid);
        JsonSerializer.Deserialize<string>(response.Data, Serialization.DefaultCamelCase).Should().Be(errorMessage);
    }

    [Fact]
    public async Task ShouldReturnNotFoundIfTriedToCallEndpointThatIsNotRegistered()
    {
        const string HandlerReturn = "Awesome, I work! But nobody calls me :(";
        var handler = () => HandlerReturn;
        _builder.MapGet(_testPath, handler);
        var broker = await _builder!.BuildAsync();
        const string notRegisteredPath = "/notRegistered";

        var guid = _builder.MockCoreWebView2.SimulateGet(notRegisteredPath);

        var response = _builder.MockCoreWebView2.ReadLastResponse();
        response.Status.Should().Be(HttpStatusCode.NotFound);
        response.RequestId.Should().Be(guid);
        JsonSerializer.Deserialize<string>(response.Data, Serialization.DefaultCamelCase).Should().Contain(notRegisteredPath);
    }

    [Theory]
    [InlineData("{\"requestId\":\"\",\"method\":\"GET\",\"path\":\"/dontCare\",\"body\":null}")]
    [InlineData("{\"requestId\":\"00000000-0000-0000-0000-000000000000\",\"method\":\"GET\",\"path\":\"/dontCare\",\"body\":null}")]
    [InlineData("{\"requestId\":null,\"method\":\"GET\",\"path\":\"/dontCare\",\"body\":null}")]
    public async Task ShouldReturnBadRequestIfNotValidGuidProvidedForRequest(string serializedRequest)
    {
        var broker = await _builder!.BuildAsync();

        _builder.MockCoreWebView2.RaiseWebMessageReceived(serializedRequest);

        var response = _builder.MockCoreWebView2.ReadLastResponse();
        response.Status.Should().Be(HttpStatusCode.BadRequest);
        response.RequestId.Should().Be(Guid.Empty);
    }

    [Theory]
    [InlineData("{\"requestId\":\"00000001-0000-0001-0000-000000000001\",\"method\":\"invalid\",\"path\":\"/dontCare\",\"body\":null}")]
    [InlineData("{\"requestId\":\"00000001-0000-0001-0000-000000000001\",\"method\":\"\",\"path\":\"/dontCare\",\"body\":null}")]
    [InlineData("{\"requestId\":\"00000001-0000-0001-0000-000000000001\",\"method\":null,\"path\":\"/dontCare\",\"body\":null}")]
    public async Task ShouldReturnBadRequestIfNotValidMethodTypeRequested(string serializedRequest)
    {
        var broker = await _builder!.BuildAsync();

        _builder.MockCoreWebView2.RaiseWebMessageReceived(serializedRequest);

        var response = _builder.MockCoreWebView2.ReadLastResponse();
        response.Status.Should().Be(HttpStatusCode.BadRequest);
        response.RequestId.Should().Be(Guid.Parse("00000001-0000-0001-0000-000000000001"));
    }

    [Theory]
    [InlineData("{\"requestId\":\"00000001-0000-0001-0000-000000000001\",\"method\":\"GET\",\"path\":\"\",\"body\":null}")]
    [InlineData("{\"requestId\":\"00000001-0000-0001-0000-000000000001\",\"method\":\"GET\",\"path\":\"  \",\"body\":null}")]
    [InlineData("{\"requestId\":\"00000001-0000-0001-0000-000000000001\",\"method\":\"GET\",\"path\":null,\"body\":null}")]
    public async Task ShouldReturnBadRequestIfNotValidPathRequested(string serializedRequest)
    {
        var broker = await _builder!.BuildAsync();

        _builder.MockCoreWebView2.RaiseWebMessageReceived(serializedRequest);

        var response = _builder.MockCoreWebView2.ReadLastResponse();
        response.Status.Should().Be(HttpStatusCode.BadRequest);
        response.RequestId.Should().Be(Guid.Parse("00000001-0000-0001-0000-000000000001"));
    }

    [Theory]
    [InlineData("This is not a valid request")]
    [InlineData("{\"RequestId\":\"00000001-0000-0001-0000-000000000001\",\"Method\":\"GET\",\"Path\":\"/valid\",\"Body\":null}")]
    [InlineData("{}")]
    [InlineData("{\"notARequestProp\":2}")]
    public async Task ShouldReturnBadRequestIfRequestIsNotValidRequest(string serializedRequest)
    {
        var broker = await _builder!.BuildAsync();

        _builder.MockCoreWebView2.RaiseWebMessageReceived(serializedRequest);

        var response = _builder.MockCoreWebView2.ReadLastResponse();
        response.Status.Should().Be(HttpStatusCode.BadRequest);
        response.RequestId.Should().Be(Guid.Empty);
    }

    [Theory]
    [ClassData(typeof(UrlParamsData))]
    public async Task ShouldPassUrlParamsToHandler<T>(string route, Func<T,T,T> handler, T expectedResult)
    {
        _builder.MapGet(route, handler);
        var broker = await _builder!.BuildAsync();

        var guid = _builder.MockCoreWebView2.SimulateGet(route);

        var response = _builder.MockCoreWebView2.ReadLastResponse();
        response.Status.Should().Be(HttpStatusCode.OK);
        response.RequestId.Should().Be(guid);
        JsonSerializer.Deserialize<T>(response.Data, Serialization.DefaultCamelCase).Should().Be(expectedResult);
    }
}

public class UrlParamsData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { "/test?param1=1&param2=2", (int param1, int param2) => param1 + param2, 3 };
        yield return new object[] { "/test?param1=1.2&param2=3.4", (double param1, double param2) => param1 + param2, 4.6 };
        yield return new object[] { "/test?param1=asd&param2=bsd", (string param1, string param2) => param1 + param2, "asdbsd" };
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}
