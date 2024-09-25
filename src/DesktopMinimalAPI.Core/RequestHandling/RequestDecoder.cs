using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using DesktopMinimalAPI.Core.RequestHandling.Models.Dtos;
using DesktopMinimalAPI.Core.RequestHandling.Models.Exceptions;
using LanguageExt;
using Microsoft.Web.WebView2.Core;
using System.Text.Json;
using static LanguageExt.Prelude;

namespace DesktopMinimalAPI.Core.RequestHandling;

internal static class RequestDecoder
{
    public static Either<RequestException, WmRequest> DecodeRequest(CoreWebView2WebMessageReceivedEventArgs e) =>
        Try(() => JsonSerializer.Deserialize<WmRequestDto>(e.WebMessageAsJson, Serialization.DefaultCamelCase) 
                        ?? throw new JsonException($"Could not deserialize {nameof(WmRequestDto)}"))
        .Match(Succ: WmRequest.From, Fail: ex => RequestException.From(ex));
}
