using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopMinimalAPI.Core.Abstractions;

public abstract class HandlerBuilderBase
{
    private readonly Dictionary<IRoute, Func<TransformedWmRequest, WmResponse>> _getMessageHandlers = [];
    private readonly Dictionary<IRoute, Func<TransformedWmRequest, Task<WmResponse>>> _asyncGetMessageHandlers = [];

    internal IReadOnlyDictionary<IRoute, Func<TransformedWmRequest, WmResponse>> GetMessageHandlers => _getMessageHandlers;
    internal IReadOnlyDictionary<IRoute, Func<TransformedWmRequest, Task<WmResponse>>> AsyncGetMessageHandlers => _asyncGetMessageHandlers;

    public abstract Task<IWebMessageBroker> BuildAsync();

    internal HandlerBuilderBase MapGet(IRoute route, Func<TransformedWmRequest, WmResponse> handler)
    {
        _getMessageHandlers.Add(route, handler);
        return this;
    }

    internal HandlerBuilderBase MapGet(IRoute route, Func<TransformedWmRequest, Task<WmResponse>> handler)
    {
        _asyncGetMessageHandlers.Add(route, handler);
        return this;
    }
}
