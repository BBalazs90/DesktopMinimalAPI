using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Core.ParameterReading;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using DesktopMinimalAPI.Models;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace DesktopMinimalAPI.Core.HandlerRegistration.EitherBased.Async;
internal static class EitherAsyncHandlerTransformer
{
    //internal static Func<WmRequest, Task<WmResponse>> Transform<TEx, TRight>(Func<Task<Either<TEx, TRight>>> handler, JsonSerializerOptions? options = null)
    //    where TEx : Exception =>
    //(request) => handler().ContinueWith(task => task.Result.Match(
    //        Right: resultValue => new WmResponse(request.Id, HttpStatusCode.OK, JsonSerializer.Serialize(resultValue, options ?? Serialization.DefaultCamelCase)),
    //        Left: ex => new WmResponse(request.Id, HttpStatusCode.InternalServerError, JsonSerializer.Serialize(ex.Message, options ?? Serialization.DefaultCamelCase))),
    //    TaskScheduler.Current);

    internal static Func<WmRequest, Task<WmResponse>> Transform<TP, TIn, TEx, TRight>(Func<TP, Task<Either<TEx, TRight>>> handler, JsonSerializerOptions? options = null) 
        where TP : ParameterSource<TIn>
        where TEx : Exception =>
      (request) =>
      {
          try
          {
              //var p1 = request.Route.Parameters.Any()
              //? TryGetParameter<TIn>(request.Route.Parameters[0], options)
              //: JsonSerializer.Deserialize<TIn>(request.Body.ValueUnsafe().Value, options ?? Serialization.DefaultCamelCase);
              var p1 = ParameterReader.GetParameter<TP,TIn>(request.Route.Parameters, request.Body.ValueUnsafe());
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

    private static T? TryGetParameter<T>(string parameter, JsonSerializerOptions? options = null)
    {
        try
        {
            return typeof(T).IsAssignableTo(typeof(IConvertible))
                ? (T)Convert.ChangeType(parameter, typeof(T), CultureInfo.InvariantCulture)
                : JsonSerializer.Deserialize<T>(parameter, options ?? Serialization.DefaultCamelCase) ?? default;
        }
        catch (InvalidCastException)
        {
            return default;
        }
        catch (FormatException)
        {
            return default;
        }
        catch (ArgumentNullException)
        {
            return default;
        }
        catch (JsonException)
        {
            return default;
        }
    }
}
