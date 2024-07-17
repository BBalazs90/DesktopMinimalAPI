using DesktopMinimalAPI.Models;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace DesktopMinimalAPI;

public sealed partial class WebMessageBrokerCore
{
    internal readonly Dictionary<IRoute, IRegisteredFunction> MessageHandlers = new();

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

        WmResponse response;
        try
        {
            var result = handler?.Invoke(request!.Body ?? string.Empty);
            response = new WmResponse(request!.RequestId, 200, result ?? string.Empty);
        }
        catch (Exception)
        {
            response = new WmResponse(request!.RequestId, 500, string.Empty);
        }

        //Application.Current.Dispatcher.Invoke(() => CoreWebView?.PostWebMessageAsString(JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })));


    }
}


public interface IRegisteredFunction
{
    public string? Invoke(string body);
}

public class ActionRegisteredFunction : IRegisteredFunction
{
    private readonly Action _action;

    public ActionRegisteredFunction(Action action)
    {
        _action = action;
    }

    public string? Invoke(string body)
    {
        _action();
        return null;
    }
}

public class ActionParamRegisteredFunction : IRegisteredFunction
{
    private readonly Action<string> _action;

    public ActionParamRegisteredFunction(Action<string> action)
    {
        _action = action;
    }

    public string? Invoke(string body)
    {
        _action(body);
        return null;
    }
}

public class FuncRegisteredFunction : IRegisteredFunction
{
    private readonly Func<string> _func;

    public FuncRegisteredFunction(Func<string> func)
    {
        _func = func;
    }

    public string? Invoke(string body) => _func();

}

public class FuncParamRegisteredFunction : IRegisteredFunction
{
    private readonly Func<string, string> _func;

    public FuncParamRegisteredFunction(Func<string, string> func)
    {
        _func = func;
    }

    public string? Invoke(string body) => _func(body);

}
