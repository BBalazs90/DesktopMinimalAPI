using Microsoft.Web.WebView2.Wpf;
using System.Threading.Tasks;

namespace DesktopMinimalAPI.WPF.Models;

public sealed class Builder
{
    internal readonly WebView2 WebView2;

    internal Builder(WebView2 webView2)
    {
        WebView2 = webView2;
    }

    public async Task<WebMessageBrokerCore> BuildAsync()
    {
        await WebView2.EnsureCoreWebView2Async();
        return new WebMessageBrokerCore(WebView2.CoreWebView2);
    }
}


