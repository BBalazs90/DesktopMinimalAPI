using DesktopMinimalAPI.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopMinimalAPI.Core.Tests.Helpers;

internal class WebMessageBrokerBuilderForTest : HandlerBuilderBase
{
    internal readonly MockCoreWebView2 MockCoreWebView2 = new();
    public override Task<WebMessageBrokerCore> BuildAsync() =>
        Task.FromResult(new WebMessageBrokerCore(MockCoreWebView2)
        {
            GetMessageHandlers = GetMessageHandlers.ToImmutableDictionary(),
            AsyncGetMessageHandlers = AsyncGetMessageHandlers.ToImmutableDictionary()
        });
}
