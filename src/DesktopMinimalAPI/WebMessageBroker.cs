using DesktopMinimalAPI.Models;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DesktopMinimalAPI;
internal class WebMessageBroker
{
    private readonly Dictionary<string, Func<BxiosResponse>> _messageHandlers = new();

    internal readonly WebView2 _webView2;
    internal Task? _initTask;

    private WebMessageBroker(WebView2 webView2)
    {
        _webView2 = webView2;
    }


    public static WebMessageBroker Create(WebView2 webView2)
    {
        return new WebMessageBroker(webView2);
    }

    internal void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        var request = JsonSerializer.Deserialize<BxiosRequest>(e.TryGetWebMessageAsString(), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
        if (_messageHandlers.TryGetValue(request.Path, out var handler))
        {
            var resposen = handler();
            var responseJson = JsonSerializer.Serialize(resposen, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
            _webView2.CoreWebView2.PostWebMessageAsString(responseJson);
        }
    }
}

internal static class WebMessageBrokerExtensions
{
    public static void SetDevServerUrl(this WebMessageBroker webMessageBroker, Uri devServerUri)
    {
        webMessageBroker._initTask = Task.Run(async () =>
        {
            await webMessageBroker._webView2.EnsureCoreWebView2Async();
            webMessageBroker._webView2.Source = devServerUri;
            webMessageBroker._webView2.CoreWebView2.WebMessageReceived += webMessageBroker.OnWebMessageReceived;
        });
    }
}
