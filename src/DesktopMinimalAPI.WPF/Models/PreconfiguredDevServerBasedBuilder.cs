using DesktopMinimalAPI.Core.Abstractions;
using DesktopMinimalAPI.Core.HandlerRegistration;
using DesktopMinimalAPI.Core.Models;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace DesktopMinimalAPI.WPF.Models;

public sealed class PreconfiguredDevServerBasedBuilder : HandlerBuilderBase
{
    private readonly WebView2 _webView2;
    private readonly string? _folderPath;
    private readonly Uri? _devServerUri;

    internal PreconfiguredDevServerBasedBuilder(WebView2 webView2, Uri devServerUri)
    {
        _webView2 = webView2;
        _devServerUri = devServerUri;
    }

    internal PreconfiguredDevServerBasedBuilder(WebView2 webView2, string folderPath)
    {
        _webView2 = webView2;
        _folderPath = folderPath;
    }

    public override async Task<IWebMessageBroker> BuildAsync()
    {
        await _webView2.EnsureCoreWebView2Async();
        if (_devServerUri is not null)
        {
            _webView2.CoreWebView2.Navigate(_devServerUri.ToString());
        }
        else if (_folderPath is not null)
        {
            _webView2.CoreWebView2.SetVirtualHostNameToFolderMapping("app",_folderPath, CoreWebView2HostResourceAccessKind.Allow);
            _webView2.CoreWebView2.Navigate("https://app/index.html");
        }
        return new WebMessageBrokerCore((CoreWebView2Wrapper)_webView2.CoreWebView2)
        {
            GetMessageHandlers = GetMessageHandlers.ToImmutableDictionary(),
            PostMessageHandlers = PostMessageHandlers.ToImmutableDictionary(),
        };
    }
}


