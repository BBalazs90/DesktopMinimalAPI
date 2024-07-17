using Microsoft.Web.WebView2.Wpf;
using DesktopMinimalAPI.WPF.Models;

namespace DesktopMinimalAPI.WPF;
public static class WebMessageBrokerWpf
{
    public static Builder CreateBuilder(WebView2 webView2) => new(webView2);
}
