using DesktopMinimalAPI.Core.Abstractions;
using System.Collections.Immutable;

namespace DesktopMinimalAPI.Core.Tests.Helpers;

internal class WebMessageBrokerBuilderForTest : HandlerBuilderBase
{
    internal readonly CoreWebView2TestInterceptor MockCoreWebView2;

    public WebMessageBrokerBuilderForTest()
    {
        MockCoreWebView2 = new();
    }

    public override Task<IWebMessageBroker> BuildAsync() =>
        Task.FromResult<IWebMessageBroker>(new WebMessageBrokerCore(MockCoreWebView2)
        {
            GetMessageHandlers = GetMessageHandlers.ToImmutableDictionary(),
            AsyncGetMessageHandlers = AsyncGetMessageHandlers.ToImmutableDictionary()
        });
}
