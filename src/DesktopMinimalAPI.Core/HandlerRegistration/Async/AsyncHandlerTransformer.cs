using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Core.ParameterReading;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using DesktopMinimalAPI.Models;
using LanguageExt.UnsafeValueAccess;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace DesktopMinimalAPI.Core.HandlerRegistration.Async;
internal static class AsyncHandlerTransformer
{
    internal static Func<WmRequest, Task<WmResponse>> Transform<T>(Func<Task<T>> handler, JsonSerializerOptions? options = null) =>
      (request) => handler().ContinueWith(t => new WmResponse(request.Id, HttpStatusCode.OK, JsonSerializer.Serialize(t.Result, options ?? Serialization.DefaultCamelCase)),
        TaskScheduler.Current);

    internal static Func<WmRequest, Task<WmResponse>> Transform<TP, TIn, TOut>(Func<TP,Task<TOut>> handler, JsonSerializerOptions? options = null)
        where TP : ParameterSource<TIn> =>
     (request) => handler(ParameterReader.GetParameter<TP, TIn>(request.Route.Parameters, request.Body.ValueUnsafe(), 0).ValueUnsafe()).ContinueWith(t => new WmResponse(request.Id, HttpStatusCode.OK, JsonSerializer.Serialize(t.Result, options ?? Serialization.DefaultCamelCase)),
       TaskScheduler.Current);

    internal static Func<WmRequest, Task<WmResponse>> Transform<TP1, TP2, TIn1, TIn2, TOut>(Func<TP1, TP2, Task<TOut>> handler, JsonSerializerOptions? options = null)
       where TP1 : ParameterSource<TIn1>
       where TP2 : ParameterSource<TIn2> =>
    (request) => handler(ParameterReader.GetParameter<TP1, TIn1>(request.Route.Parameters, request.Body.ValueUnsafe(), 0).ValueUnsafe(),
        ParameterReader.GetParameter<TP2, TIn2>(request.Route.Parameters, request.Body.ValueUnsafe(), 1).ValueUnsafe())
    .ContinueWith(t => new WmResponse(request.Id, HttpStatusCode.OK, JsonSerializer.Serialize(t.Result, options ?? Serialization.DefaultCamelCase)),
      TaskScheduler.Current);

}
