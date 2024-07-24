using DesktopMinimalAPI.Core.Abstractions;

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
