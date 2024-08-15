﻿using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.Models.Dtos;
using DesktopMinimalAPI.Models;
using LanguageExt;
using Microsoft.Web.WebView2.Core;
using System;
using System.Net;
using System.Text.Json;

namespace DesktopMinimalAPI.Core.RequestHandling;

internal static class RequestReaderPipeline
{
    public static Either<Exception, WmRequestType> DecodeRequest(EventArgs e) =>
        WmRequest.From(JsonSerializer.Deserialize<WmRequestDto>(GetWebMessageAsString(e), Serialization.DefaultCamelCase));



    //{
    //    try
    //    {
    //        var requestDto = JsonSerializer.Deserialize<WmRequestDto>(GetWebMessageAsString(e), Serialization.DefaultCamelCase);
    //        return (WmRequestBuilder.BuildFrom(requestDto), null);
    //    }
    //    catch (JsonException ex)
    //    {
    //        return TryGetRequestId(e, out var id)
    //            ? (null, new WmResponse(id, HttpStatusCode.BadRequest, ex.Message))
    //            : (null, new WmResponse(Guid.Empty, HttpStatusCode.BadRequest, ex.Message));
    //    }
    //    catch (ArgumentException ex)
    //    {
    //        return TryGetRequestId(e, out var id)
    //            ? (null, new WmResponse(id, HttpStatusCode.BadRequest, ex.Message))
    //            : (null, new WmResponse(Guid.Empty, HttpStatusCode.BadRequest, ex.Message));
    //    }
    //}

    private static Either<JsonException, WmRequestDto> TryDeserialize(string json) =>
        JsonSerializer.Deserialize<WmRequestDto>(json, Serialization.DefaultCamelCase) is WmRequestDto dto
        ? dto
        : new JsonException("Could not deserialize the request.");

    private static string GetWebMessageAsString(EventArgs e) =>
           // This required for testing purposes, since CoreWebView2WebMessageReceivedEventArgs has no public ctr, thus not possible to simulate
           // the even fire. Therefore reflection magic is needed, but this overhead is eliminated in prod.
#if DEBUG
           e is CoreWebView2WebMessageReceivedEventArgs cwvArg
           ? cwvArg.WebMessageAsJson
           : (e.GetType().GetProperty("WebMessageAsJson")?.GetValue(e) as string ?? throw new ArgumentException("The provided type has no WebMessageAsJson property"));
#else
        ((CoreWebView2WebMessageReceivedEventArgs)e).WebMessageAsJson;
#endif

    private static bool TryGetRequestId(EventArgs e, out Guid requestId)
    {
        try
        {
            var guidPart = JsonSerializer.Deserialize<GuidPartOfRequest>(GetWebMessageAsString(e), Serialization.DefaultCamelCase);
            requestId = guidPart is null ? Guid.Empty : guidPart.RequestId;
            return guidPart is not null;
        }
        catch (JsonException)
        {
            requestId = Guid.Empty;
            return false;
        }
    }

    class GuidPartOfRequest(Guid requestId)
    {
        public Guid RequestId { get; } = requestId;
    }
}
