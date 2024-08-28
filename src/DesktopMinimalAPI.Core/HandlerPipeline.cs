using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using DesktopMinimalAPI.Models;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace DesktopMinimalAPI.Core;

[SuppressMessage("Design", "CA1031: Do not catch general exception types",
    Justification = "The handler method can throw any kind of exception, and this " +
    "must be cached and returned to the requestor.")]
internal static class HandlerPipeline
{
    internal static Func<WmRequest, WmResponse> Transform<T>(Func<T> handler, JsonSerializerOptions? options = null) =>
      (request) =>
      {
          try
          {
              var result = handler();
              return new WmResponse(request.Id, HttpStatusCode.OK, JsonSerializer.Serialize(result, options ?? Serialization.DefaultCamelCase));
          }
          catch (Exception ex)
          {
              return new WmResponse(request.Id, HttpStatusCode.InternalServerError, JsonSerializer.Serialize(ex.Message, options ?? Serialization.DefaultCamelCase));
          }
      };

    //internal static Func<TransformedWmRequest, WmResponse> Transform<T>(Func<Task<T>> handler, JsonSerializerOptions? options = null) =>
    //  (request) =>
    //  {
    //      try
    //      {
    //          var result = handler().GetAwaiter().GetResult();
    //          return new WmResponse(request.Id, HttpStatusCode.OK, JsonSerializer.Serialize(result, options ?? Serialization.DefaultCamelCase));
    //      }
    //      catch (Exception ex)
    //      {
    //          return new WmResponse(request.Id, HttpStatusCode.InternalServerError, JsonSerializer.Serialize(ex.Message, options ?? Serialization.DefaultCamelCase));
    //      }
    //  };

    public static Func<WmRequest, WmResponse> Transform<TIn, TOut>(Func<TIn, TOut> handler, JsonSerializerOptions? options = null) =>
      (request) =>
      {
          try
          {
              var p1 = TryGetParameter<TIn>(request.Route.Parameters[0], options);
              var result = handler(p1);
              return new WmResponse(request.Id, HttpStatusCode.OK, JsonSerializer.Serialize(result, options ?? Serialization.DefaultCamelCase));
          }
          catch (Exception ex)
          {
              return new WmResponse(request.Id, HttpStatusCode.InternalServerError, JsonSerializer.Serialize(ex.Message, options ?? Serialization.DefaultCamelCase));
          }
      };

    public static Func<WmRequest, WmResponse> Transform<TIn1, TIn2, TOut>(Func<TIn1, TIn2, TOut> handler, JsonSerializerOptions? options = null) =>
      (request) =>
      {
          try
          {
              var p1 = TryGetParameter<TIn1>(request.Route.Parameters[0], options);
              var p2 = TryGetParameter<TIn2>(request.Route.Parameters[1], options);
              var result = handler(p1, p2);
              return new WmResponse(request.Id, HttpStatusCode.OK, JsonSerializer.Serialize(result, options ?? Serialization.DefaultCamelCase));
          }
          catch (Exception ex)
          {
              return new WmResponse(request.Id, HttpStatusCode.InternalServerError, JsonSerializer.Serialize(ex.Message, options ?? Serialization.DefaultCamelCase));
          }
      };

    public static Func<WmRequest, WmResponse> Transform<TIn1, TIn2, TIn3, TOut>(Func<TIn1, TIn2, TIn3, TOut> handler, JsonSerializerOptions? options = null) =>
     (request) =>
     {
         try
         {
             var p1 = TryGetParameter<TIn1>(request.Route.Parameters[0], options);
             var p2 = TryGetParameter<TIn2>(request.Route.Parameters[1], options);
             var p3 = TryGetParameter<TIn3>(request.Route.Parameters[2], options);
             var result = handler(p1, p2, p3);
             return new WmResponse(request.Id, HttpStatusCode.OK, JsonSerializer.Serialize(result, options ?? Serialization.DefaultCamelCase));
         }
         catch (Exception ex)
         {
             return new WmResponse(request.Id, HttpStatusCode.InternalServerError, JsonSerializer.Serialize(ex.Message, options ?? Serialization.DefaultCamelCase));
         }
     };

    public static Func<WmRequest, Task<WmResponse>> Transform<T>(Func<Task<T>> handler, JsonSerializerOptions? options = null) =>
      (request) =>
      {
          try
          {
              var result = handler();
              return result
              .ContinueWith(t => new WmResponse(request.Id, HttpStatusCode.OK, JsonSerializer.Serialize(t.Result, options ?? Serialization.DefaultCamelCase)),
              TaskScheduler.Current);
          }
          catch (Exception ex)
          {
              return Task.FromResult(new WmResponse(request.Id, HttpStatusCode.InternalServerError, JsonSerializer.Serialize(ex, options ?? Serialization.DefaultCamelCase)));
          }
      };

    public static Func<WmRequest, Task<WmResponse>> Transform<TIn, TOut>(Func<TIn, Task<TOut>> handler, JsonSerializerOptions? options = null) =>
      (request) =>
      {
          try
          {
              throw new NotImplementedException();
              //var p1 = TryGetParameter<TIn>(request.ParameterInfos[0], options);
              //var result = handler(p1);
              //return result
              //.ContinueWith(t => new WmResponse(request.Id, HttpStatusCode.OK, JsonSerializer.Serialize(t.Result, options ?? Serialization.DefaultCamelCase)),
              //TaskScheduler.Current);
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

