using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopMinimalAPI.WPF;
public static class WebMessageBrokerWpf
{
    public static Builder CreateBuilder(WebView2 webView2) => new(webView2);

    public sealed class Builder
    {
        private readonly WebView2 _webView2;
        internal Uri? DevServerUri;

        internal Builder(WebView2 webView2)
        {
            _webView2 = webView2;
        }

        public async Task<WebMessageBrokerCore> BuildAsync()
        {
            await _webView2.EnsureCoreWebView2Async();
            _webView2.CoreWebView2.Navigate(DevServerUri?.ToString() ?? "about:blank");
            return new WebMessageBrokerCore(_webView2.CoreWebView2);
        }
    }
}

public static class WebMessageBrokerWpfExtenions
{
    public static WebMessageBrokerWpf.Builder SetDevServerUrl(this WebMessageBrokerWpf.Builder builder, Uri devServerUri)
    {
        builder.DevServerUri = devServerUri;

        return builder;
    }
}

public sealed partial class WebMessageBrokerCore
{
    internal readonly CoreWebView2 CoreWebView;

    internal WebMessageBrokerCore(CoreWebView2 coreWebView)
    {
        CoreWebView = coreWebView;
    }
}
