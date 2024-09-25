using DesktopMinimalAPI.Core.Abstractions;
using Microsoft.Web.WebView2.Core;
using System;

namespace DesktopMinimalAPI.Core.Models;
public sealed class CoreWebView2Wrapper : ICoreWebView2
{
    private readonly CoreWebView2 _coreWebView2;

    public CoreWebView2Wrapper(CoreWebView2 coreWebView2)
    {
        _coreWebView2 = coreWebView2;
        _coreWebView2.WebMessageReceived += (sender, e) => WebMessageReceived?.Invoke(sender, e);
    }

    public event EventHandler<CoreWebView2WebMessageReceivedEventArgs>? WebMessageReceived;

    public void PostWebMessageAsString(string webMessageAsString) 
        => _coreWebView2.PostWebMessageAsString(webMessageAsString);

    public static CoreWebView2Wrapper ToCoreWebView2Wrapper(CoreWebView2 coreWebView2) => new(coreWebView2);

    public static explicit operator CoreWebView2Wrapper(CoreWebView2 coreWebView2) => ToCoreWebView2Wrapper(coreWebView2);

}
