using DesktopMinimalAPI.Models;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopMinimalAPI;
public class WebMessageBrokerCore
{
    private readonly Dictionary<string, Func<BxiosResponse>> _messageHandlers = new();

    internal Task? InitTask;
    internal CoreWebView2? CoreWebView;

    internal WebMessageBrokerCore() { }


    internal void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        var request = JsonSerializer.Deserialize<BxiosRequest>(e.WebMessageAsJson, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        if (_messageHandlers.TryGetValue(request.Path, out var handler))
        {
            var resposen = handler();
            var responseJson = JsonSerializer.Serialize(resposen, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            if (CoreWebView is null) throw new InvalidOperationException();
            CoreWebView?.PostWebMessageAsString(responseJson);
        }
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
            webMessageBroker.InitTask.ContinueWith(t =>
            {
                Debug.Assert(webMessageBroker.CoreWebView is not null, "Create extension method must have already initialized this property.");
                webMessageBroker.CoreWebView.Navigate(devServerUri.ToString());
            }, TaskContinuationOptions.ExecuteSynchronously);
        }

        return webMessageBroker;

        
    }
}
