using DesktopMinimalAPI.Core.Abstractions;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopMinimalAPI.Core.Tests.Helpers;

internal class MockCoreWebView2 : ICoreWebView2
{
    public event EventHandler<CoreWebView2WebMessageReceivedEventArgs> WebMessageReceived;

    public void PostWebMessageAsString(string webMessageAsString)
    {
        throw new NotImplementedException();
    }
}
