using DesktopMinimalAPI.Core.Abstractions;
using DesktopMinimalAPI.Core.Models;
using Microsoft.Web.WebView2.Wpf;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace DesktopMinimalAPI.WPF.Models;

public sealed class Builder : HandlerBuilderBase
{
    internal readonly WebView2 WebView2;

    internal Builder(WebView2 webView2)
    {
        WebView2 = webView2;
    }

    public override async Task<WebMessageBrokerCore> BuildAsync()
    {
        await WebView2.EnsureCoreWebView2Async();
        return new WebMessageBrokerCore((CoreWebView2Wrapper)WebView2.CoreWebView2)
        {
            GetMessageHandlers = GetMessageHandlers.ToImmutableDictionary(),
            AsyncGetMessageHandlers = AsyncGetMessageHandlers.ToImmutableDictionary()
        };
    }
}


