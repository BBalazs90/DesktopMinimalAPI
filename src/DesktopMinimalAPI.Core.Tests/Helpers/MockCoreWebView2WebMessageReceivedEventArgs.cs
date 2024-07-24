namespace DesktopMinimalAPI.Core.Tests.Helpers;

public class MockCoreWebView2WebMessageReceivedEventArgs(string WebMessageAsJson) : EventArgs
{
    public string WebMessageAsJson { get; } = WebMessageAsJson;
}
