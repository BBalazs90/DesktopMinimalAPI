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

    public event EventHandler<EventArgs>? WebMessageReceived;

    public void PostWebMessageAsString(string webMessageAsString) 
        => _coreWebView2.PostWebMessageAsString(webMessageAsString);

    public static explicit operator CoreWebView2Wrapper(CoreWebView2 coreWebView2) => new CoreWebView2Wrapper(coreWebView2);
}
