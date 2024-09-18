using static DesktopMinimalAPI.Core.HandlerRegistration.Sync.SyncHandlerTransformer;
using static DesktopMinimalAPI.Core.HandlerRegistration.HandlerResult;
using static DesktopMinimalAPI.Core.Support.SerializationHelper;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using LanguageExt.UnsafeValueAccess;
using DesktopMinimalAPI.Core.RequestHandling.Models.Methods;
using LanguageExt;
using FluentAssertions;
using System.Net;
using DesktopMinimalAPI.Core.HandlerRegistration;

namespace DesktopMinimalAPI.Core.Tests;
public class WhenHandlerTransformed
{
    private const string TestRouteString = "/test";
    private static readonly Route TestRoute = Route.From(TestRouteString).Value();
    private static readonly RequestId TestRequestId = RequestId.From(Guid.NewGuid().ToString()).Value();
    private static readonly WmRequest TestRequest = new(TestRequestId, Method.Get, TestRoute, Option<JsonBody>.None);

    [Theory]
    [MemberData(nameof(GetHandlerResultData))]
    public void ShouldReturnAFunctionThatProperlyRepresentsResult(Func<HandlerResult<object>> handler,
        HttpStatusCode expectedStatusCode, string expectedData)
    {
        var transformedHandler = Transform(handler);

        var response = transformedHandler(TestRequest);

        _ = response.RequestId.Should().Be(TestRequestId);
        _ = response.Status.Should().Be(expectedStatusCode);
        _ = response.Data.Should().Be(expectedData);
    }

    public static IEnumerable<object[]> GetHandlerResultData =>
        new List<object[]>
        {
            new object[] { () => Ok(), HttpStatusCode.OK, SerializeCamelCase(new object()) },
            new object[] { () => BadRequest<object>("Bad Request"), HttpStatusCode.BadRequest, SerializeCamelCase(new {Message= "Bad Request"}) },
            new object[] { () => InternalServerError<object>("Internal Server Error"), HttpStatusCode.InternalServerError, SerializeCamelCase(new { Message = "Internal Server Error" }) }
        };

   
}
