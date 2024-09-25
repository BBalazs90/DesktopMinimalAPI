using Microsoft.Web.WebView2.Core;
using System;

namespace DesktopMinimalAPI.Core.Abstractions;

public interface ICoreWebView2
{
    public event EventHandler<CoreWebView2WebMessageReceivedEventArgs>? WebMessageReceived;

    public void PostWebMessageAsString(string webMessageAsString);
}
