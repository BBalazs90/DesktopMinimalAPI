using DesktopMinimalAPI.WPF.Models;
using System;

namespace DesktopMinimalAPI.WPF.Extensions;

public static class WebMessageBrokerWpfExtenions
{
    public static PreconfiguredDevServerBasedBuilder SetDevServerUrl(this Builder builder, Uri devServerUri) =>
        new(builder.WebView2, devServerUri);
}


