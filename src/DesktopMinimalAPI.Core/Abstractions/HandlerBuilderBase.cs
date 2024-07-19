using DesktopMinimalAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopMinimalAPI.Core.Abstractions;

public abstract class HandlerBuilderBase
{
    private readonly Dictionary<IRoute, Func<WmRequest, WmResponse>> _getMessageHandlers = [];
    private readonly Dictionary<IRoute, Func<WmRequest, Task<WmResponse>>> _asyncGetMessageHandlers = [];

    public IReadOnlyDictionary<IRoute, Func<WmRequest, WmResponse>> GetMessageHandlers => _getMessageHandlers;
    public IReadOnlyDictionary<IRoute, Func<WmRequest, Task<WmResponse>>> AsyncGetMessageHandlers => _asyncGetMessageHandlers;

    public abstract Task<WebMessageBrokerCore> BuildAsync();

    public HandlerBuilderBase MapGet(IRoute route, Func<WmRequest, WmResponse> handler)
    {
        _getMessageHandlers.Add(route, handler);
        return this;
    }

    public HandlerBuilderBase MapGet(IRoute route, Func<WmRequest, Task<WmResponse>> handler)
    {
        _asyncGetMessageHandlers.Add(route, handler);
        return this;
    }
}
