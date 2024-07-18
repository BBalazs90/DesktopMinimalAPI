using DesktopMinimalAPI.Models;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace DesktopMinimalAPI;

public sealed partial class WebMessageBrokerCore
{
    internal readonly Dictionary<IRoute, Func<WmRequest, WmResponse>> MessageHandlers = new();

    public readonly CoreWebView2 CoreWebView;

    public WebMessageBrokerCore(CoreWebView2 coreWebView)
    {
        CoreWebView = coreWebView;
        CoreWebView.WebMessageReceived += OnWebMessageReceived;
    }


    internal void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        var request = JsonSerializer.Deserialize<WmRequest>(e.WebMessageAsJson, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        if (request is null) return;

        var handler = request.Method switch
        {
            Methods.Get => MessageHandlers.GetValueOrDefault((StringRoute)(request.Path)),
            _ => null
        };

        var response = handler?.Invoke(request);


        CoreWebView?.PostWebMessageAsString(JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));


    }

    public void MapGet(IRoute route, Func<WmRequest, WmResponse> handler)
    {
        MessageHandlers.Add(route, handler);
    }

    public void MapGet(string route, Func<WmRequest, WmResponse> handler) => MapGet((StringRoute)route, handler);
    public void MapGet(string route, Action handler) => MapGet((StringRoute)route, handler.ToWebMessageCommunicationForm());

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
}

public static class ActionAndFuncExtensions
{
    public static Func<WmRequest, WmResponse> ToWebMessageCommunicationForm(this Action action) => HandlerPipeline.Transform(action);
}
