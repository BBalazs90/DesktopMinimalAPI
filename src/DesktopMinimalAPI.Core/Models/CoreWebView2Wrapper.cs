using DesktopMinimalAPI.Core.Abstractions;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopMinimalAPI.Core.Models;
public sealed class CoreWebView2Wrapper : ICoreWebView2
{
    private readonly CoreWebView2 _coreWebView2;

    public CoreWebView2Wrapper(CoreWebView2 coreWebView2)
    {
        _coreWebView2 = coreWebView2;
    }

    // Define a new event in your wrapper class
    public event EventHandler<CoreWebView2WebMessageReceivedEventArgs> WebMessageReceived
    {
        add { _coreWebView2.WebMessageReceived += value;}
        remove { _coreWebView2.WebMessageReceived -= value;}
    }

    public void PostWebMessageAsString(string webMessageAsString) 
        => _coreWebView2.PostWebMessageAsString(webMessageAsString);

    public static explicit operator CoreWebView2Wrapper(CoreWebView2 coreWebView2) => new CoreWebView2Wrapper(coreWebView2);
}
