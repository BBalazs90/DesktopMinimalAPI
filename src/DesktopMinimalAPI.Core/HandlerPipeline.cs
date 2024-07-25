using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Models;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace DesktopMinimalAPI;

public static class HandlerPipeline
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

    public static Func<WmRequest, WmResponse> Transform<T>(Func<T> handler, JsonSerializerOptions? options = null) =>
      (request) =>
      {
          try
          {
              var result = handler();
              return new WmResponse(request.RequestId, HttpStatusCode.OK, JsonSerializer.Serialize(result, options ?? Serialization.DefaultCamelCase));
          }
          catch (Exception ex)
          {
              return new WmResponse(request.RequestId, HttpStatusCode.InternalServerError, JsonSerializer.Serialize(ex.Message, options ?? Serialization.DefaultCamelCase));
          }
      };

    public static Func<WmRequest, Task<WmResponse>> Transform<T>(Func<Task<T>> handler, JsonSerializerOptions? options = null) =>
      (request) =>
      {
          try
          {
              var result = handler();
              return result.ContinueWith(t => new WmResponse(request.RequestId, HttpStatusCode.OK, JsonSerializer.Serialize(t.Result, options ?? Serialization.DefaultCamelCase)));
          }
          catch (Exception ex)
          {
              return Task.FromResult(new WmResponse(request.RequestId, HttpStatusCode.InternalServerError, JsonSerializer.Serialize(ex, options ?? Serialization.DefaultCamelCase)));
          }
      };
}

