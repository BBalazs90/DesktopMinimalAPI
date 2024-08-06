using System;

namespace DesktopMinimalAPI.Core.Abstractions;

public interface ICoreWebView2
{
    public event EventHandler<EventArgs>? WebMessageReceived;

    public void PostWebMessageAsString(string webMessageAsString);
}
