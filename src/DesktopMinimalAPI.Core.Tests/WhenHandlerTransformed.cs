﻿using static DesktopMinimalAPI.Core.HandlerRegistration.Sync.SyncHandlerTransformer;
using static DesktopMinimalAPI.Core.HandlerRegistration.HandlerResult;
using static DesktopMinimalAPI.Core.Support.SerializationHelper;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using LanguageExt.UnsafeValueAccess;
using DesktopMinimalAPI.Core.RequestHandling.Models.Methods;
using LanguageExt;
using FluentAssertions;
using System.Net;
using DesktopMinimalAPI.Core.HandlerRegistration;
using DesktopMinimalAPI.Core.Models;

namespace DesktopMinimalAPI.Core.Tests;
public class WhenHandlerTransformed
{
    private const string TestRouteString = "/test";
    private static readonly Route TestRoute = Route.From(TestRouteString).Value();
    private static readonly RequestId TestRequestId = RequestId.From(Guid.NewGuid().ToString()).Value();
    private static readonly WmRequest TestRequest = new(TestRequestId, Method.Get, TestRoute, Option<JsonBody>.None);

    [Theory]
    [MemberData(nameof(GetHandlerResultDataWithoutParameter))]
    public void ShouldReturnAFunctionThatProperlyRepresentsResult<T>(Func<HandlerResult<T>> handler,
        HttpStatusCode expectedStatusCode, string expectedData)
    {
        var transformedHandler = Transform(handler);

        var response = transformedHandler(TestRequest);

        _ = response.RequestId.Should().Be(TestRequestId);
        _ = response.Status.Should().Be(expectedStatusCode);
        _ = response.Data.Should().Be(expectedData);
    }

    [Theory]
    [MemberData(nameof(GetHandlerResultDataWithOneUrlParameter))]
    public void ShouldReturnASingleParameterFunctionThatProperlyRepresentsResult<TIn,TOut>(Func<FromUrl<TIn>,HandlerResult<TOut>> handler,
        string urlString, HttpStatusCode expectedStatusCode, string expectedData)
    {
        var transformedHandler = Transform(handler);
        var route = Route.From(urlString).Value();
        var request = new WmRequest(TestRequestId, Method.Get, route, Option<JsonBody>.None);

        var response = transformedHandler(request);

        _ = response.RequestId.Should().Be(TestRequestId);
        _ = response.Status.Should().Be(expectedStatusCode);
        _ = response.Data.Should().Be(expectedData);
    }

    public static IEnumerable<object[]> GetHandlerResultDataWithoutParameter =>
        new List<object[]>
        {
            new object[] { () => Ok(), HttpStatusCode.OK, SerializeCamelCase(new object()) },
            new object[] { () => Ok(new TestRecord("Balazs", 34)), HttpStatusCode.OK, SerializeCamelCase(new TestRecord("Balazs", 34)) },
            new object[] { () => BadRequest<TestRecord>("Bad Request"), HttpStatusCode.BadRequest, SerializeCamelCase(new {Message= "Bad Request"}) },
            new object[] { () => BadRequest<TestRecord>(), HttpStatusCode.BadRequest, SerializeCamelCase(new {Message= ""}) },
            new object[] { () => InternalServerError<TestRecord>("Internal Server Error"), HttpStatusCode.InternalServerError, SerializeCamelCase(new { Message = "Internal Server Error" }) },
            new object[] { () => InternalServerError<TestRecord>(), HttpStatusCode.InternalServerError, SerializeCamelCase(new { Message = "" }) },
            new object[] { () => { throw new Exception("I am nasty and dishonest!"); return Ok(); }, HttpStatusCode.BadRequest, SerializeCamelCase(new { Message = "I am nasty and dishonest!" }) }
        };

    public static IEnumerable<object[]> GetHandlerResultDataWithOneUrlParameter =>
        new List<object[]>
        {
            new object[] { (FromUrl<string> s) => Ok(), "/test?p1=asd", HttpStatusCode.OK, SerializeCamelCase(new object()) },
            new object[] { (FromUrl<int> i) => Ok(), "/test?p1=1", HttpStatusCode.OK, SerializeCamelCase(new object()) },
            new object[] { (FromUrl<string> s) => BadRequest<string>("Bad Request"), "/test?p1=asd", HttpStatusCode.BadRequest, SerializeCamelCase(new {Message= "Bad Request"}) },
            new object[] { (FromUrl<string> s) => InternalServerError<string>("Internal Server Error"), "/test?p1=asd", HttpStatusCode.InternalServerError, SerializeCamelCase(new { Message = "Internal Server Error" }) },
            new object[] { (FromUrl<string> s) => { throw new Exception("I am nasty and dishonest!"); return Ok(); }, "/test?p1=asd", HttpStatusCode.BadRequest, SerializeCamelCase(new { Message = "I am nasty and dishonest!" }) }
        };

    public sealed record TestRecord(string Name, int Age);
}
