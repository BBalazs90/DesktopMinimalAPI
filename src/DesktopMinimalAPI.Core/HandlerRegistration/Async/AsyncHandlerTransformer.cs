using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Core.ParameterReading;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using DesktopMinimalAPI.Models;
using LanguageExt.UnsafeValueAccess;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace DesktopMinimalAPI.Core.HandlerRegistration.Async;
internal static class AsyncHandlerTransformer
{
    internal static Func<WmRequest, Task<WmResponse>> Transform<T>(Func<Task<HandlerResult<T>>> handler, JsonSerializerOptions? options = null) =>
      (request) => handler()
        .ContinueWith(task => new WmResponse(request.Id,
                                            task.Result.StatusCode, 
                                            task.Result.Value.Match<string>(
                                                Left: msg => msg, 
                                                Right: value => JsonSerializer.Serialize(value, options ?? Serialization.DefaultCamelCase))),
        TaskScheduler.Current);

    internal static Func<WmRequest, Task<WmResponse>> Transform<TIn, TOut>(Func<FromUrl<TIn>, Task<HandlerResult<TOut>>> handler, JsonSerializerOptions? options = null) =>
     (request) => handler(ParameterReader.GetUrlParameter<TIn>(request.Route.Parameters).ValueUnsafe())
     .ContinueWith(task => new WmResponse(request.Id, task.Result.StatusCode, task.Result.Value.Match<string>(
                                                Left: msg => msg,
                                                Right: value => JsonSerializer.Serialize(value, options ?? Serialization.DefaultCamelCase))),
       TaskScheduler.Current);

    internal static Func<WmRequest, Task<WmResponse>> Transform<TIn1, TIn2, TOut>(Func<FromUrl<TIn1>, FromUrl<TIn2>, Task<HandlerResult<TOut>>> handler, JsonSerializerOptions? options = null) =>
    (request) =>
    {
        var (p1, p2) = ParameterReader.GetUrlParameters<TIn1, TIn2>(request.Route.Parameters).ValueUnsafe();
        return handler(p1, p2)
        .ContinueWith(task => new WmResponse(request.Id, task.Result.StatusCode, JsonSerializer.Serialize(task.Result.Value, options ?? Serialization.DefaultCamelCase)),
                 TaskScheduler.Current);
    };

    internal static Func<WmRequest, Task<WmResponse>> Transform<TIn, TOut>(Func<FromBody<TIn>, Task<HandlerResult<TOut>>> handler, JsonSerializerOptions? options = null) =>
     (request) => handler(ParameterReader.GetBodyParameter<TIn>(request.Body.ValueUnsafe()).ValueUnsafe())
     .ContinueWith(task => new WmResponse(request.Id, task.Result.StatusCode, task.Result.Value.Match<string>(
                                                Left: msg => msg,
                                                Right: value => JsonSerializer.Serialize(value, options ?? Serialization.DefaultCamelCase))),
       TaskScheduler.Current);
}
