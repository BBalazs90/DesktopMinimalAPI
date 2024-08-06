using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Models;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace DesktopMinimalAPI.Core;

internal static class HandlerPipeline
{
    internal static Func<TransformedWmRequest, WmResponse> Transform<T>(Func<T> handler, JsonSerializerOptions? options = null) =>
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

    public static Func<TransformedWmRequest, WmResponse> Transform<TIn, TOut>(Func<TIn, TOut> handler, JsonSerializerOptions? options = null) =>
      (request) =>
      {
          try
          {
              var p1 = TryGetParameter<TIn>(request.ParameterInfos[0], options);
              var result = handler(p1);
              return new WmResponse(request.Id, HttpStatusCode.OK, JsonSerializer.Serialize(result, options ?? Serialization.DefaultCamelCase));
          }
          catch (Exception ex)
          {
              return new WmResponse(request.Id, HttpStatusCode.InternalServerError, JsonSerializer.Serialize(ex.Message, options ?? Serialization.DefaultCamelCase));
          }
      };

    public static Func<TransformedWmRequest, WmResponse> Transform<TIn1, TIn2, TOut>(Func<TIn1, TIn2, TOut> handler, JsonSerializerOptions? options = null) =>
      (request) =>
      {
          try
          {
              var p1 = TryGetParameter<TIn1>(request.ParameterInfos[0], options);
              var p2 = TryGetParameter<TIn2>(request.ParameterInfos[1], options);
              var result = handler(p1, p2);
              return new WmResponse(request.Id, HttpStatusCode.OK, JsonSerializer.Serialize(result, options ?? Serialization.DefaultCamelCase));
          }
          catch (Exception ex)
          {
              return new WmResponse(request.Id, HttpStatusCode.InternalServerError, JsonSerializer.Serialize(ex.Message, options ?? Serialization.DefaultCamelCase));
          }
      };

    public static Func<TransformedWmRequest, Task<WmResponse>> Transform<T>(Func<Task<T>> handler, JsonSerializerOptions? options = null) =>
      (request) =>
      {
          try
          {
              var result = handler();
              return result.ContinueWith(t => new WmResponse(request.Id, HttpStatusCode.OK, JsonSerializer.Serialize(t.Result, options ?? Serialization.DefaultCamelCase)));
          }
          catch (Exception ex)
          {
              return Task.FromResult(new WmResponse(request.Id, HttpStatusCode.InternalServerError, JsonSerializer.Serialize(ex, options ?? Serialization.DefaultCamelCase)));
          }
      };

    public static Func<TransformedWmRequest, Task<WmResponse>> Transform<TIn, TOut>(Func<TIn, Task<TOut>> handler, JsonSerializerOptions? options = null) =>
      (request) =>
      {
          try
          {
              var p1 = TryGetParameter<TIn>(request.ParameterInfos[0], options);
              var result = handler(p1);
              return result.ContinueWith(t => new WmResponse(request.Id, HttpStatusCode.OK, JsonSerializer.Serialize(t.Result, options ?? Serialization.DefaultCamelCase)));
          }
          catch (Exception ex)
          {
              return Task.FromResult(new WmResponse(request.Id, HttpStatusCode.InternalServerError, JsonSerializer.Serialize(ex, options ?? Serialization.DefaultCamelCase)));
          }
      };

    private static T TryGetParameter<T>(RequestParameterIntermediate parameter, JsonSerializerOptions? options = null)
    {
        try
        {
            return typeof(T).IsAssignableTo(typeof(IConvertible)) ? (T)Convert.ChangeType(parameter.SerializedParameter, typeof(T)) : default;
        }
        catch (Exception ex)
        {
            return default;
        }
    }
}

