using DesktopMinimalAPI.Models;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopMinimalAPI;

public sealed partial class WebMessageBrokerCore
{
    internal readonly Dictionary<IRoute, Func<WmRequest, WmResponse>> MessageHandlers = new();
    internal readonly Dictionary<IRoute, Func<WmRequest, Task<WmResponse>>> AsyncMessageHandlers = new();

    internal readonly JsonSerializerOptions _options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public readonly CoreWebView2 CoreWebView;
    private readonly SynchronizationContext? _context;

    public WebMessageBrokerCore(CoreWebView2 coreWebView)
    {
        CoreWebView = coreWebView;
        CoreWebView.WebMessageReceived += OnWebMessageReceived;
        _context = SynchronizationContext.Current;
    }


    internal void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        var request = JsonSerializer.Deserialize<WmRequest>(e.WebMessageAsJson, _options);
        if (request is null) return;

        var handler = request.Method switch
        {
            Methods.Get => MessageHandlers.GetValueOrDefault((StringRoute)(request.Path)),
            _ => null
        };

        if (handler is null)
        {
            var asyncHandler = request.Method switch
            {
                Methods.Get => AsyncMessageHandlers.GetValueOrDefault((StringRoute)(request.Path)),
                _ => null
            };

            Task.Run(async () => await asyncHandler?
                .Invoke(request)
                .ContinueWith(resp =>
            {
                _context.Post(_ =>
                {
                    CoreWebView?.PostWebMessageAsString(JsonSerializer.Serialize(resp.Result, _options));
                }, null);

            }));

            return;
        }

        var response = handler?.Invoke(request);


        CoreWebView?.PostWebMessageAsString(JsonSerializer.Serialize(response, _options));


    }

    public void MapGet(IRoute route, Func<WmRequest, WmResponse> handler)
    {
        MessageHandlers.Add(route, handler);           
    }

    public void MapGet(IRoute route, Func<WmRequest, Task<WmResponse>> handler)
    {
        AsyncMessageHandlers.Add(route, handler);
    }

    public void MapGet(string route, Func<WmRequest, WmResponse> handler) => MapGet((StringRoute)route, handler);
    public void MapGet(string route, Action handler) => MapGet((StringRoute)route, HandlerPipeline.Transform( handler));
    public void MapGet<T>(string route, Func<T> handler) => MapGet((StringRoute)route, HandlerPipeline.Transform(handler));
    public void MapGet<T>(string route, Func<Task<T>> handler) => MapGet((StringRoute)route, HandlerPipeline.Transform(handler, _options));

}

public static class HandlerPipeline
{
    public static Func<WmRequest, WmResponse> Transform(Action handler) =>
        (request) =>
        {
            try
            {
                handler();
                return new WmResponse(request.RequestId, 200, string.Empty);
            }
            catch (Exception ex)
            {
                return new WmResponse(request.RequestId, 500, JsonSerializer.Serialize(ex));
            }
        };

    public static Func<WmRequest, WmResponse> Transform<T>(Action<T> handler) =>
       (request) =>
       {
           try
           {
               var param = JsonSerializer.Deserialize<T>(request.Body);
               handler(param);
               return new WmResponse(request.RequestId, 200, string.Empty);
           }
           catch (Exception ex)
           {
               return new WmResponse(request.RequestId, 500, JsonSerializer.Serialize(ex));
           }
       };

    public static Func<WmRequest, WmResponse> Transform<T>(Func<T> handler) =>
      (request) =>
      {
          try
          {
              var result = handler();
              return new WmResponse(request.RequestId, 200, JsonSerializer.Serialize(result));
          }
          catch (Exception ex)
          {
              return new WmResponse(request.RequestId, 500, JsonSerializer.Serialize(ex));
          }
      };

    public static Func<WmRequest, Task<WmResponse>> Transform<T>(Func<Task<T>> handler, JsonSerializerOptions options) =>
      (request) =>
      {
          try
          {
              var result = handler();
              return result.ContinueWith(t => new WmResponse(request.RequestId, 200, JsonSerializer.Serialize(t.Result, options)));
          }
          catch (Exception ex)
          {
              return Task.FromResult(new WmResponse(request.RequestId, 500, JsonSerializer.Serialize(ex, options)));
          }
      };
}

