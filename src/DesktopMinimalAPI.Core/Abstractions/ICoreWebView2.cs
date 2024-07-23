using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DesktopMinimalAPI.Core.Abstractions;

public interface ICoreWebView2
{
    public event EventHandler<CoreWebView2WebMessageReceivedEventArgs> WebMessageReceived;

    public void PostWebMessageAsString(string webMessageAsString);

}
