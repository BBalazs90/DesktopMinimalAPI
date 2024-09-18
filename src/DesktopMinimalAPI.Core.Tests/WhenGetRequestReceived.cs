using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.Tests.Helpers;
using System.Text.Json;
using FluentAssertions;
using System.Net;
using DesktopMinimalAPI.Core.HandlerRegistration.Sync;
using DesktopMinimalAPI.Core.HandlerRegistration.Async;
using LanguageExt;

using static DesktopMinimalAPI.Core.HandlerRegistration.HandlerResult;
using DesktopMinimalAPI.Core.HandlerRegistration;
using DesktopMinimalAPI.Core.Models;

namespace DesktopMinimalAPI.Core.Tests;

public sealed class WhenGetRequestReceived
{
    private readonly WebMessageBrokerBuilderForTest _builder;
    private const string _testPath = "/test";

    public WhenGetRequestReceived()
    {
        _builder = new WebMessageBrokerBuilderForTest();
    }

    [Fact]
    public async Task ShouldReturnOkResponseWithHandlerContentAndRequestId()
    {
        const string SimulatedResult = "Awesome, I work!";
        var handler = () => Ok(SimulatedResult);
        _ = _builder.MapGet(_testPath, handler);
        _ = await _builder!.BuildAsync();

        var guid = _builder.MockCoreWebView2.SimulateGet(_testPath);

        var response = await _builder.MockCoreWebView2.ReadLastResponseAsync();
        _ = response.Status.Should().Be(HttpStatusCode.OK);
        _ = response.RequestId.Should().Be(guid);
        _ = JsonSerializer.Deserialize<string>(response.Data, Serialization.DefaultCamelCase).Should().Be(SimulatedResult);
    }

    [Fact]
    public async Task ShouldReturnErrorResponseAndTheExceptionIfHandlerFailed()
    {
        const string SimulatedErrorMessage = "Oh, noooo!";
        Func<HandlerResult<int>> handler = () => throw new Exception(SimulatedErrorMessage); // Explicit type because from exception cannot be infered.
        _ = _builder.MapGet(_testPath, handler);
        _ = await _builder!.BuildAsync();

        var guid = _builder.MockCoreWebView2.SimulateGet(_testPath);

        var response = await _builder.MockCoreWebView2.ReadLastResponseAsync();
        _ = response.Status.Should().Be(HttpStatusCode.BadRequest);
        _ = response.RequestId.Should().Be(guid);
        _ = JsonSerializer.Deserialize<string>(response.Data, Serialization.DefaultCamelCase).Should().Be(SimulatedErrorMessage);
    }

    [Fact]
    public async Task ShouldReturnErrorResponseWithHandlerContentIfHandlerReturnedError()
    {
        const string SimulatedResult = "Oh noo, I could not succeed!";
        var handler = () => BadRequest(SimulatedResult);
        _ = _builder.MapGet(_testPath, handler);
        _ = await _builder!.BuildAsync();

        var guid = _builder.MockCoreWebView2.SimulateGet(_testPath);

        var response = await _builder.MockCoreWebView2.ReadLastResponseAsync();
        _ = response.Status.Should().Be(HttpStatusCode.BadRequest);
        _ = response.RequestId.Should().Be(guid);
        _ = JsonSerializer.Deserialize<string>(response.Data, Serialization.DefaultCamelCase).Should().Be(SimulatedResult);
    }

    [Fact]
    public async Task ShouldReturnNotFoundIfTriedToCallEndpointThatIsNotRegistered()
    {
        const string HandlerReturn = "Awesome, I work! But nobody calls me :(";
        var handler = () => Ok(HandlerReturn);
        _ = _builder.MapGet(_testPath, handler);
        _ = await _builder!.BuildAsync();
        const string notRegisteredPath = "/notRegistered";

        var guid = _builder.MockCoreWebView2.SimulateGet(notRegisteredPath);

        var response = await _builder.MockCoreWebView2.ReadLastResponseAsync();
        _ = response.Status.Should().Be(HttpStatusCode.NotFound);
        _ = response.RequestId.Should().Be(guid);
        _ = response.Data.Should().Contain(notRegisteredPath);
    }

    [Theory]
    [InlineData("{\"requestId\":\"\",\"method\":\"GET\",\"path\":\"/dontCare\",\"body\":null}")]
    [InlineData("{\"requestId\":\"00000000-0000-0000-0000-000000000000\",\"method\":\"GET\",\"path\":\"/dontCare\",\"body\":null}")]
    [InlineData("{\"requestId\":null,\"method\":\"GET\",\"path\":\"/dontCare\",\"body\":null}")]
    public async Task ShouldReturnBadRequestIfNotValidGuidProvidedForRequest(string serializedRequest)
    {
        _ = await _builder!.BuildAsync();

        _builder.MockCoreWebView2.RaiseWebMessageReceived(serializedRequest);

        var response = await _builder.MockCoreWebView2.ReadLastResponseAsync();
        _ = response.Status.Should().Be(HttpStatusCode.BadRequest);
        _ = response.RequestId.Should().Be(Guid.Empty);
    }

    [Theory]
    [InlineData("{\"requestId\":\"00000001-0000-0001-0000-000000000001\",\"method\":\"invalid\",\"path\":\"/dontCare\",\"body\":null}")]
    [InlineData("{\"requestId\":\"00000001-0000-0001-0000-000000000001\",\"method\":\"\",\"path\":\"/dontCare\",\"body\":null}")]
    [InlineData("{\"requestId\":\"00000001-0000-0001-0000-000000000001\",\"method\":null,\"path\":\"/dontCare\",\"body\":null}")]
    public async Task ShouldReturnBadRequestIfNotValidMethodTypeRequested(string serializedRequest)
    {
        _ = await _builder!.BuildAsync();

        _builder.MockCoreWebView2.RaiseWebMessageReceived(serializedRequest);

        var response = await _builder.MockCoreWebView2.ReadLastResponseAsync();
        _ = response.Status.Should().Be(HttpStatusCode.BadRequest);
        _ = response.RequestId.Should().Be(Guid.Parse("00000001-0000-0001-0000-000000000001"));
    }

    [Theory]
    [InlineData("{\"requestId\":\"00000001-0000-0001-0000-000000000001\",\"method\":\"GET\",\"path\":\"\",\"body\":null}")]
    [InlineData("{\"requestId\":\"00000001-0000-0001-0000-000000000001\",\"method\":\"GET\",\"path\":\"  \",\"body\":null}")]
    [InlineData("{\"requestId\":\"00000001-0000-0001-0000-000000000001\",\"method\":\"GET\",\"path\":null,\"body\":null}")]
    public async Task ShouldReturnBadRequestIfNotValidPathRequested(string serializedRequest)
    {
        _ = await _builder!.BuildAsync();

        _builder.MockCoreWebView2.RaiseWebMessageReceived(serializedRequest);

        var response = await _builder.MockCoreWebView2.ReadLastResponseAsync();
        _ = response.Status.Should().Be(HttpStatusCode.BadRequest);
        _ = response.RequestId.Should().Be(Guid.Parse("00000001-0000-0001-0000-000000000001"));
    }

    [Theory]
    [InlineData("This is not a valid request")]
    [InlineData("{\"RequestId\":\"00000001-0000-0001-0000-000000000001\",\"Method\":\"GET\",\"Path\":\"/valid\",\"Body\":null}")]
    [InlineData("{}")]
    [InlineData("{\"notARequestProp\":2}")]
    public async Task ShouldReturnBadRequestIfRequestIsNotValidRequest(string serializedRequest)
    {
        _ = await _builder!.BuildAsync();

        _builder.MockCoreWebView2.RaiseWebMessageReceived(serializedRequest);

        var response = await _builder.MockCoreWebView2.ReadLastResponseAsync();
        _ = response.Status.Should().Be(HttpStatusCode.BadRequest);
        _ = response.RequestId.Should().Be(Guid.Empty);
    }


    [Theory]
    [ClassData(typeof(UrlParamData))]
    public async Task ShouldPassUrlParamToHandler<TIn, TOut>(string route, Func<FromUrl<TIn>, HandlerResult<TOut>> handler, TOut expectedResult)
    {
 
        _ = _builder.MapGet(route, handler);
        _ = await _builder!.BuildAsync();

        var guid = _builder.MockCoreWebView2.SimulateGet(route);

        var response = await _builder.MockCoreWebView2.ReadLastResponseAsync();
        _ = response.Status.Should().Be(HttpStatusCode.OK);
        _ = response.RequestId.Should().Be(guid);
        _ = JsonSerializer.Deserialize<TOut>(response.Data, Serialization.DefaultCamelCase).Should().Be(expectedResult);
    }


    [Theory]
    [ClassData(typeof(BodyParamData))]
    public async Task ShouldPassBodyParamToHandler<TIn, TOut>(TIn param, Func<TIn, TOut> handler, TOut expectedResult)
    {
        throw new NotImplementedException();
        //_ = _builder.MapGet(_testPath, handler);
        //_ = await _builder!.BuildAsync();

        //var guid = _builder.MockCoreWebView2.SimulateGet(_testPath, JsonSerializer.Serialize(param, Serialization.DefaultCamelCase));

        //var response = await _builder.MockCoreWebView2.ReadLastResponseAsync();
        //_ = response.Status.Should().Be(HttpStatusCode.OK);
        //_ = response.RequestId.Should().Be(guid);
        //_ = JsonSerializer.Deserialize<TOut>(response.Data, Serialization.DefaultCamelCase).Should().Be(expectedResult);
    }

    [Theory]
    [ClassData(typeof(BodyParamsData))]
    public async Task ShouldPassBodyParamsToHandlerAndAssumeThatTheBodyIsAStringDictionary<TIn1, TIn2, TOut>(Dictionary<string, string> body, Func<TIn1, TIn2, TOut> handler, TOut expectedResult)
    {
        throw new NotImplementedException();
        //_ = _builder.MapGet(_testPath, handler);
        //_ = await _builder!.BuildAsync();

        //var guid = _builder.MockCoreWebView2.SimulateGet(_testPath, JsonSerializer.Serialize(body, Serialization.DefaultCamelCase));

        //var response = await _builder.MockCoreWebView2.ReadLastResponseAsync();
        //_ = response.Status.Should().Be(HttpStatusCode.OK);
        //_ = response.RequestId.Should().Be(guid);
        //_ = JsonSerializer.Deserialize<TOut>(response.Data, Serialization.DefaultCamelCase).Should().Be(expectedResult);
    }



    [Theory]
    [ClassData(typeof(Url2ParamsData))]
    public async Task ShouldPassUrlParamsToHandler<T>(string route, Func<T, T, T> handler, T expectedResult)
    {
        throw new NotImplementedException();
        //_ = _builder.MapGet(route, handler);
        //_ = await _builder!.BuildAsync();

        //var guid = _builder.MockCoreWebView2.SimulateGet(route);

        //var response = await _builder.MockCoreWebView2.ReadLastResponseAsync();
        //_ = response.Status.Should().Be(HttpStatusCode.OK);
        //_ = response.RequestId.Should().Be(guid);
        //_ = JsonSerializer.Deserialize<T>(response.Data, Serialization.DefaultCamelCase).Should().Be(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnResponseResultAsBodyEvenIfHandlerReturnsTask()
    {
        throw new NotImplementedException();
        //var testBody = new TestBody("Balazs", 34);
        //var handler = async () => await Task.FromResult(testBody);
        //_ = _builder.MapGet(_testPath, handler);
        //_ = await _builder!.BuildAsync();

        //var guid = _builder.MockCoreWebView2.SimulateGet(_testPath);

        //var response = await _builder.MockCoreWebView2.ReadLastResponseAsync();
        //_ = response.Status.Should().Be(HttpStatusCode.OK);
        //_ = response.RequestId.Should().Be(guid);
        //_ = JsonSerializer.Deserialize<TestBody>(response.Data, Serialization.DefaultCamelCase).Should().Be(testBody);
    }
}

public class UrlParamData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { "/test?param1=1", (FromUrl<int> param1) => Ok(2 * param1), 2 * 1 };
        yield return new object[] { "/test?param1=1", (FromUrl<int> param1) => Ok(2.3 * param1), 2.3 * 1 };
        yield return new object[] { "/test?param1=1.2", (FromUrl<double> param1) => Ok(2 * param1), 2 * 1.2 };
        yield return new object[] { "/test?param1=1.2", (FromUrl<double> param1) => Ok((int)Math.Floor(param1)), 1 };
        yield return new object[] { "/test?param1=asd", (FromUrl<string> param1) => Ok(param1 + "l"), "asdl" };
        //yield return new object[] { "/test?param1=asd", (FromUrl<string> param1) => Ok(param1.Length), 3 };
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}

public static class FuncExt
{
    public static Func<ParameterSource<TIn>, HandlerResult<TOut>> Ass<TIn, TOut>(this Func<FromUrl<TIn>, HandlerResult<TOut>> source) =>
        source as Func<ParameterSource<TIn>, HandlerResult<TOut>> ?? throw new ArgumentOutOfRangeException("Nooooooo");
}

public class BodyParamData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { new UserPwd("bbalazs", "secretPass"), (UserPwd userPwd) => userPwd.User + userPwd.Password, "bbalazssecretPass" };

    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}

public class UserPwd(string user, string password)
{
    public string User { get; } = user;
    public string Password { get; } = password;
}

public class BodyParamsData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { new Dictionary<string, string> { { "p1", "123"}, { "p2", "345" } }, (int p1, int p2) => p1+p2, 123+345 };

    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}

public class UrlParamsDataAsync : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { "/test?param1=1", (int param1) => Task.FromResult(2 * param1), 2 * 1 };
        yield return new object[] { "/test?param1=1", (int param1) => Task.FromResult(2.3 * param1), 2.3 * 1 };
        yield return new object[] { "/test?param1=1.2", (double param1) => Task.FromResult(2 * param1), 2 * 1.2 };
        yield return new object[] { "/test?param1=1.2", (double param1) => Task.FromResult((int)Math.Floor(param1)), 1 };
        yield return new object[] { "/test?param1=asd", (string param1) => Task.FromResult(param1 + "l"), "asdl" };
        yield return new object[] { "/test?param1=asd", (string param1) => Task.FromResult(param1.Length), 3 };
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}

public class Url2ParamsData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { "/test?param1=1&param2=2", (int param1, int param2) => param1 + param2, 3 };
        yield return new object[] { "/test?param1=1.2&param2=3.4", (double param1, double param2) => param1 + param2, 4.6 };
        yield return new object[] { "/test?param1=asd&param2=bsd", (string param1, string param2) => param1 + param2, "asdbsd" };
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}

internal record TestBody(string Name, int Age);
