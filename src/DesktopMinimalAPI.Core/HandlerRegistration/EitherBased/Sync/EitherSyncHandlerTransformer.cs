using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using DesktopMinimalAPI.Models;
using LanguageExt;
using System;
using System.Net;
using System.Text.Json;

namespace DesktopMinimalAPI.Core.HandlerRegistration.EitherBased.Sync;
internal static class EitherSyncHandlerTransformer
{
    internal static Func<WmRequest, WmResponse> Transform<Tex, TRight>(Func<Either<Tex, TRight>> handler, JsonSerializerOptions? options = null)
        where Tex : Exception =>
     (request) => handler().Match(
             Right: resultValue => new WmResponse(request.Id, HttpStatusCode.OK, JsonSerializer.Serialize(resultValue, options ?? Serialization.DefaultCamelCase)),
             Left: ex => new WmResponse(request.Id, HttpStatusCode.InternalServerError, JsonSerializer.Serialize(ex.Message, options ?? Serialization.DefaultCamelCase)));

}
