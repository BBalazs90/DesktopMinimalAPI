﻿using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Core.ParameterReading;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using DesktopMinimalAPI.Models;
using LanguageExt.UnsafeValueAccess;
using System;
using System.Net;
using System.Text.Json;
using static LanguageExt.Prelude;
using static DesktopMinimalAPI.Core.Support.SerializationHelper;

namespace DesktopMinimalAPI.Core.HandlerRegistration.Sync;
internal static class SyncHandlerTransformer
{
   
    internal static Func<WmRequest, WmResponse> Transform<T>(Func<HandlerResult<T>> handler) =>
      (request) => Try(handler)
        .Match(
            Succ: result => new WmResponse(request.Id, result.StatusCode, CreateResponseBody(result)),
            Fail: ex => new WmResponse(request.Id, HttpStatusCode.BadRequest, CreateResponseBody(ex)));


    public static Func<WmRequest, WmResponse> Transform<TIn, TOut>(Func<FromUrl<TIn>, HandlerResult<TOut>> handler, JsonSerializerOptions? options = null) =>
        (request) =>
        {
            try
            {
                var p1 = ParameterReader.GetUrlParameter<TIn>(request.Route.Parameters);
                var result = handler(p1.ValueUnsafe());
                return new WmResponse(request.Id, result.StatusCode, JsonSerializer.Serialize(result.Value, options ?? Serialization.DefaultCamelCase));
            }
            catch (Exception ex)
            {
                return new WmResponse(request.Id, HttpStatusCode.InternalServerError, JsonSerializer.Serialize(ex.Message, options ?? Serialization.DefaultCamelCase));
            }
        };

    public static Func<WmRequest, WmResponse> Transform<TIn1, TIn2, TOut>(Func<FromUrl<TIn1>, FromUrl<TIn2>, HandlerResult<TOut>> handler, JsonSerializerOptions? options = null) =>
        (request) =>
        {
            try
            {
                var (p1, p2) = ParameterReader.GetUrlParameters<TIn1, TIn2>(request.Route.Parameters).ValueUnsafe();
                var result = handler(p1, p2);
                return new WmResponse(request.Id, result.StatusCode, JsonSerializer.Serialize(result.Value, options ?? Serialization.DefaultCamelCase));
            }
            catch (Exception ex)
            {
                return new WmResponse(request.Id, HttpStatusCode.InternalServerError, JsonSerializer.Serialize(ex.Message, options ?? Serialization.DefaultCamelCase));
            }
        };

    private static string CreateResponseBody<T>(HandlerResult<T> result) =>
       result.Value.Match<string>(
                      Left: msg => SerializeCamelCase(new {Message = (string)msg}),
                      Right: value => SerializeCamelCase(value));

    private static string CreateResponseBody(Exception ex) =>
        SerializeCamelCase(new {Message = ex.Message});
}
