using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Core.ParameterReading;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using DesktopMinimalAPI.Models;
using LanguageExt.UnsafeValueAccess;
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


    public static Func<WmRequest, WmResponse> Transform<TP, TIn, TOut>(Func<TP, TOut> handler, JsonSerializerOptions? options = null)
       where TP : ParameterSource<TIn> =>
        (request) =>
        {
            try
            {
                var p1 = ParameterReader.GetParameter<TP, TIn>(request.Route.Parameters, request.Body.ValueUnsafe(), 0);
                var result = handler(p1.ValueUnsafe());
                return new WmResponse(request.Id, HttpStatusCode.OK, JsonSerializer.Serialize(result, options ?? Serialization.DefaultCamelCase));
            }
            catch (Exception ex)
            {
                return new WmResponse(request.Id, HttpStatusCode.InternalServerError, JsonSerializer.Serialize(ex.Message, options ?? Serialization.DefaultCamelCase));
            }
        };

    public static Func<WmRequest, WmResponse> Transform<TP1, TP2, TIn1, TIn2, TOut>(Func<TP1, TP2, TOut> handler, JsonSerializerOptions? options = null)
              where TP1 : ParameterSource<TIn1>
              where TP2 : ParameterSource<TIn2> =>
               (request) =>
               {
                   try
                   {
                       var p1 = ParameterReader.GetParameter<TP1, TIn1>(request.Route.Parameters, request.Body.ValueUnsafe(), 0);
                       var p2 = ParameterReader.GetParameter<TP2, TIn2>(request.Route.Parameters, request.Body.ValueUnsafe(), 1);
                       var result = handler(p1.ValueUnsafe(), p2.ValueUnsafe());
                       return new WmResponse(request.Id, HttpStatusCode.OK, JsonSerializer.Serialize(result, options ?? Serialization.DefaultCamelCase));
                   }
                   catch (Exception ex)
                   {
                       return new WmResponse(request.Id, HttpStatusCode.InternalServerError, JsonSerializer.Serialize(ex.Message, options ?? Serialization.DefaultCamelCase));
                   }
               };
}
