using DesktopMinimalAPI.Models;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopMinimalAPI;
public class WebMessageBrokerCore
{
    internal readonly Dictionary<IRoute, IRegisteredFunction> MessageHandlers = new();

    internal Task? InitTask;
    internal CoreWebView2? CoreWebView;

    internal WebMessageBrokerCore() { }


    internal void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        var request = JsonSerializer.Deserialize<BxiosRequest>(e.WebMessageAsJson, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        if (request is null) return;

        var handler = request.Method switch
        {
            Methods.Get => MessageHandlers.GetValueOrDefault((StringRoute)(request.Path)),
            _ => null
        };

        BxiosResponse response;
        try
        {
            var result = handler?.Invoke(request!.Body ?? string.Empty);
            response = new BxiosResponse(request!.RequestId, 200, result ?? string.Empty);
        }
        catch (Exception)
        {
            response = new BxiosResponse(request!.RequestId, 500, string.Empty);
        }

        Application.Current.Dispatcher.Invoke(() => CoreWebView?.PostWebMessageAsString(JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })));


    }
}

public static class WebMessageBrokerCoreExtensions
{
    public static WebMessageBrokerCore MapGet(this WebMessageBrokerCore webMessageBroker, string route, Action handler) => webMessageBroker.MapGetInternal((StringRoute)route, new ActionRegisteredFunction(handler));
    public static WebMessageBrokerCore MapGet(this WebMessageBrokerCore webMessageBroker, string route, Action<string> handler) => webMessageBroker.MapGetInternal((StringRoute)route, new ActionParamRegisteredFunction(handler));
    public static WebMessageBrokerCore MapGet(this WebMessageBrokerCore webMessageBroker, string route, Func<string> handler) => webMessageBroker.MapGetInternal((StringRoute)route, new FuncRegisteredFunction(handler));
    public static WebMessageBrokerCore MapGet(this WebMessageBrokerCore webMessageBroker, string route, Func<string, string> handler) => webMessageBroker.MapGetInternal((StringRoute)route, new FuncParamRegisteredFunction(handler));
    private static WebMessageBrokerCore MapGetInternal(this WebMessageBrokerCore webMessageBroker, IRoute route, IRegisteredFunction handler)
    {
        webMessageBroker.MessageHandlers.Add(route, handler);
        return webMessageBroker;
    }
}

public static class WebMessageBrokerWpf
{
    public static WebMessageBrokerCore Create(WebView2 webView2)
    {
        var broker = new WebMessageBrokerCore();
        broker.InitTask = Init(webView2);

        return broker;


        async Task Init(WebView2 wv)
        {
            await wv.EnsureCoreWebView2Async();
            broker.CoreWebView = wv.CoreWebView2;
            broker.CoreWebView.WebMessageReceived += broker.OnWebMessageReceived;
        }
    }

    public static WebMessageBrokerCore SetDevServerUrl(this WebMessageBrokerCore webMessageBroker, Uri devServerUri)
    {
        if (webMessageBroker.InitTask is null) throw new InvalidOperationException($"{nameof(webMessageBroker)} was not properly initialized!");

        if (webMessageBroker.InitTask.IsCompletedSuccessfully)
        {
            Debug.Assert(webMessageBroker.CoreWebView is not null, "Create extension method must have already initialized this property.");
            webMessageBroker.CoreWebView.Navigate(devServerUri.ToString());
        }
        else
        {
            webMessageBroker.InitTask = webMessageBroker.InitTask.ContinueWith(t =>
            {
                Debug.Assert(webMessageBroker.CoreWebView is not null, "Create extension method must have already initialized this property.");
                webMessageBroker.CoreWebView.Navigate(devServerUri.ToString());
            },
            TaskContinuationOptions.ExecuteSynchronously);
        }

        return webMessageBroker;


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
