using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using DesktopMinimalAPI.Models;
using System;
using System.Net;
using System.Text.Json;
using static LanguageExt.Prelude;

namespace DesktopMinimalAPI.Core.HandlerRegistration.Sync;
internal static class SyncHandlerTransformer
{
    internal static Func<WmRequest, WmResponse> Transform<T>(Func<T> handler, JsonSerializerOptions? options = null) =>
      (request) => Try(handler)
        .Match(
            Succ: result => new WmResponse(request.Id, HttpStatusCode.OK, JsonSerializer.Serialize(result, options ?? Serialization.DefaultCamelCase)),
            Fail: ex => new WmResponse(request.Id, HttpStatusCode.InternalServerError, JsonSerializer.Serialize(ex.Message, options ?? Serialization.DefaultCamelCase)));
     
}
