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
    //public static Func<WmRequest, WmResponse> Transform(Action handler) =>
    //    (request) =>
    //    {
    //        try
    //        {
    //            handler();
    //            return new WmResponse(request.RequestId, 200, string.Empty);
    //        }
    //        catch (Exception ex)
    //        {
    //            return new WmResponse(request.RequestId, 500, JsonSerializer.Serialize(ex));
    //        }
    //    };

    //public static Func<WmRequest, WmResponse> Transform<T>(Action<T> handler) =>
    //   (request) =>
    //   {
    //       try
    //       {
    //           var param = JsonSerializer.Deserialize<T>(request.Body);
    //           handler(param);
    //           return new WmResponse(request.RequestId, 200, string.Empty);
    //       }
    //       catch (Exception ex)
    //       {
    //           return new WmResponse(request.RequestId, 500, JsonSerializer.Serialize(ex));
    //       }
    //};

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

    public static Func<TransformedWmRequest, WmResponse> Transform<Tin, Tout>(Func<Tin, Tout> handler, JsonSerializerOptions? options = null) =>
      (request) =>
      {
          try
          {
              var result = handler(default);
              return new WmResponse(request.Id, HttpStatusCode.OK, JsonSerializer.Serialize(result, options ?? Serialization.DefaultCamelCase));
          }
          catch (Exception ex)
          {
              return new WmResponse(request.Id, HttpStatusCode.InternalServerError, JsonSerializer.Serialize(ex.Message, options ?? Serialization.DefaultCamelCase));
          }
      };

    public static Func<TransformedWmRequest, WmResponse> Transform<Tin1, Tin2, Tout>(Func<Tin1, Tin2, Tout> handler, JsonSerializerOptions? options = null) =>
      (request) =>
      {
          try
          {
              var type = handler.GetType();
              var result = handler(default, default);
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
}

