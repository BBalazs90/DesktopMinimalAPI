using DesktopMinimalAPI.Core.Abstractions;
using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Models;
using System.Text.Json;

namespace DesktopMinimalAPI.Core.Tests.Helpers;

internal class CoreWebView2TestInterceptor : ICoreWebView2
{
    public event EventHandler<EventArgs>? WebMessageReceived;

    public string LastPostedWebMessageAsString { get; private set; } = string.Empty;

    public void PostWebMessageAsString(string webMessageAsString) => LastPostedWebMessageAsString = webMessageAsString;

    public void RaiseWebMessageReceived(string message)
    {
        var arg = new MockCoreWebView2WebMessageReceivedEventArgs(message);
        WebMessageReceived?.Invoke(this, arg);
    }
}

internal static class CoreWebView2TestInterceptorExtensions
{
    public static Guid SimulateGet(this CoreWebView2TestInterceptor webView, string path)
    {
        var guid = Guid.NewGuid();
        webView.RaiseWebMessageReceived(JsonSerializer.Serialize(new WmRequest(guid, (Methods)"GET", path), Serialization.DefaultCamelCase));
        return guid;
    }

    public static WmResponse ReadLastResponse(this CoreWebView2TestInterceptor webView) => 
        JsonSerializer.Deserialize<WmResponse>(webView.LastPostedWebMessageAsString, Serialization.DefaultCamelCase)
        ?? throw new InvalidOperationException($"Could not deserialize the last webmessage. Its content: '{webView.LastPostedWebMessageAsString}'");
}
