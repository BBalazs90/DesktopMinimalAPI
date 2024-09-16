using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Core.ParameterReading;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using DesktopMinimalAPI.Models;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace DesktopMinimalAPI.Core.HandlerRegistration.EitherBased.Async;
internal static class EitherAsyncHandlerTransformer
{
    internal static Func<WmRequest, Task<WmResponse>> Transform<TEx, TRight>(Func<Task<Either<TEx, TRight>>> handler, JsonSerializerOptions? options = null)
        where TEx : Exception =>
    (request) => handler().ContinueWith(task => task.Result.Match(
            Right: resultValue => new WmResponse(request.Id, HttpStatusCode.OK, JsonSerializer.Serialize(resultValue, options ?? Serialization.DefaultCamelCase)),
            Left: ex => new WmResponse(request.Id, HttpStatusCode.InternalServerError, JsonSerializer.Serialize(ex.Message, options ?? Serialization.DefaultCamelCase))),
        TaskScheduler.Current);

    internal static Func<WmRequest, Task<WmResponse>> Transform<TP, TIn, TEx, TRight>(Func<TP, Task<Either<TEx, TRight>>> handler, JsonSerializerOptions? options = null) 
        where TP : ParameterSource<TIn>
        where TEx : Exception =>
      (request) =>
      {
          try
          {
              var p1 = ParameterReader.GetParameter<TP,TIn>(request.Route.Parameters, request.Body.ValueUnsafe(), 0);
              var result = handler(p1.ValueUnsafe());
              return result
              .ContinueWith(task => task.Result.Match(
                    Right: resultValue => new WmResponse(request.Id, HttpStatusCode.OK, JsonSerializer.Serialize(resultValue, options ?? Serialization.DefaultCamelCase)),
                    Left: ex => new WmResponse(request.Id, HttpStatusCode.InternalServerError, JsonSerializer.Serialize(ex.Message, options ?? Serialization.DefaultCamelCase))),
                TaskScheduler.Current);
          }
          catch (Exception ex)
          {
              return Task.FromResult(new WmResponse(request.Id, HttpStatusCode.InternalServerError, JsonSerializer.Serialize(ex, options ?? Serialization.DefaultCamelCase)));
          }
      };

    internal static Func<WmRequest, Task<WmResponse>> Transform<TP1, TP2, TIn1, TIn2, TEx, TRight>(Func<TP1, TP2, Task<Either<TEx, TRight>>> handler, JsonSerializerOptions? options = null)
        where TP1 : ParameterSource<TIn1>
        where TP2 : ParameterSource<TIn2>
        where TEx : Exception =>
      (request) =>
      {
          try
          {
              var p1 = ParameterReader.GetParameter<TP1, TIn1>(request.Route.Parameters, request.Body.ValueUnsafe(), 0);
              var p2 = ParameterReader.GetParameter<TP2, TIn2>(request.Route.Parameters, request.Body.ValueUnsafe(), 1);
              var result = handler(p1.ValueUnsafe(), p2.ValueUnsafe());
              return result
              .ContinueWith(task => task.Result.Match(
                    Right: resultValue => new WmResponse(request.Id, HttpStatusCode.OK, JsonSerializer.Serialize(resultValue, options ?? Serialization.DefaultCamelCase)),
                    Left: ex => new WmResponse(request.Id, HttpStatusCode.InternalServerError, JsonSerializer.Serialize(ex.Message, options ?? Serialization.DefaultCamelCase))),
                TaskScheduler.Current);
          }
          catch (Exception ex)
          {
              return Task.FromResult(new WmResponse(request.Id, HttpStatusCode.InternalServerError, JsonSerializer.Serialize(ex, options ?? Serialization.DefaultCamelCase)));
          }
      };
}
