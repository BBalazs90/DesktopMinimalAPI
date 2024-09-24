using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Core.ParameterReading;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using DesktopMinimalAPI.Models;
using System;
using System.Net;
using System.Text.Json;
using LanguageExt;
using static LanguageExt.Prelude;
using static DesktopMinimalAPI.Core.HandlerRegistration.HandlerResult;
using static DesktopMinimalAPI.Core.HandlerRegistration.ResponseHelper;
using static DesktopMinimalAPI.Core.ParameterReading.ParameterReader;

namespace DesktopMinimalAPI.Core.HandlerRegistration.Sync;
internal static class SyncHandlerTransformer
{
   
    internal static Func<WmRequest, WmResponse> Transform<T>(Func<HandlerResult<T>> handler) =>
      (request) => Try(handler)
        .Match(
            Succ: result => new WmResponse(request.Id, result.StatusCode, CreateResponseBody(result)),
            Fail: ex => new WmResponse(request.Id, HttpStatusCode.BadRequest, CreateResponseBody(ex)));


    internal static Func<WmRequest, WmResponse> Transform<TIn, TOut>(Func<FromUrl<TIn>, HandlerResult<TOut>> handler) =>
        (request) =>
                GetUrlParameter<TIn>(request.Route.Parameters)
                .Match(
                       Some: p => Try(() => handler(p)).IfFail(ex => BadRequest<TOut>(ex.Message)),
                       None: BadRequest<TOut>("Could not find the required URL parameter."))
                .Apply(result => new WmResponse(request.Id, result.StatusCode, CreateResponseBody(result)));
            

    internal static Func<WmRequest, WmResponse> Transform<TIn1, TIn2, TOut>(Func<FromUrl<TIn1>, FromUrl<TIn2>, HandlerResult<TOut>> handler, JsonSerializerOptions? options = null) =>
        (request) =>
                GetUrlParameters<TIn1, TIn2>(request.Route.Parameters)
                .Match(
                       Some: parameters => Try(() => handler(parameters.Item1, parameters.Item2)).IfFail(ex => BadRequest<TOut>(ex.Message)),
                       None: BadRequest<TOut>("Could not find the required URL parameter."))
                .Apply(result => new WmResponse(request.Id, result.StatusCode, CreateResponseBody(result)));
       
}
