using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using DesktopMinimalAPI.Core.RequestHandling.Models.Dtos;
using DesktopMinimalAPI.Core.RequestHandling.Models.Exceptions;
using LanguageExt;
using Microsoft.Web.WebView2.Core;
using System;
using System.Net;
using System.Text.Json;

namespace DesktopMinimalAPI.Core.RequestHandling;

internal static class RequestReaderPipeline
{
    public static Either<RequestException, WmRequest> DecodeRequest(EventArgs e) =>
            new Try<WmRequestDto>(() => JsonSerializer.Deserialize<WmRequestDto>(GetWebMessageAsString(e), Serialization.DefaultCamelCase) ?? throw new JsonException($"Could not deserialize {nameof(WmRequestDto)}"))
        .Match(Succ: WmRequest.From, Fail: ex => RequestException.From(ex));


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
}
