using DesktopMinimalAPI.Core.Abstractions;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace DesktopMinimalAPI.WPF.Models;

public sealed class PreconfiguredDevServerBasedBuilder : HandlerBuilderBase
{
    private readonly WebView2 _webView2;
    private readonly Uri _devServerUri;

    internal PreconfiguredDevServerBasedBuilder(WebView2 webView2, Uri devServerUri)
    {
        _webView2 = webView2;
        _devServerUri = devServerUri;
    }

    public override async Task<WebMessageBrokerCore> BuildAsync()
    {
        await _webView2.EnsureCoreWebView2Async();
        _webView2.CoreWebView2.Navigate(_devServerUri.ToString());
        return new WebMessageBrokerCore(_webView2.CoreWebView2)
        {
            GetMessageHandlers = GetMessageHandlers.ToImmutableDictionary(),
            AsyncGetMessageHandlers = AsyncGetMessageHandlers.ToImmutableDictionary()
        }; 
    }
}


