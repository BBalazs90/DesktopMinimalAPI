using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopMinimalAPI.Core.Abstractions;

public abstract class HandlerBuilderBase
{
    private readonly Dictionary<IRoute, Func<TransformedWmRequest, WmResponse>> _getMessageHandlers = [];
    private readonly Dictionary<IRoute, Func<TransformedWmRequest, WmResponse>> _postMessageHandlers = [];
    private readonly Dictionary<IRoute, Func<TransformedWmRequest, Task<WmResponse>>> _asyncGetMessageHandlers = [];
    private readonly Dictionary<IRoute, Func<TransformedWmRequest, Task<WmResponse>>> _asyncPostMessageHandlers = [];

    internal IReadOnlyDictionary<IRoute, Func<TransformedWmRequest, WmResponse>> GetMessageHandlers => _getMessageHandlers;
    internal IReadOnlyDictionary<IRoute, Func<TransformedWmRequest, WmResponse>> PostMessageHandlers => _postMessageHandlers;
    internal IReadOnlyDictionary<IRoute, Func<TransformedWmRequest, Task<WmResponse>>> AsyncGetMessageHandlers => _asyncGetMessageHandlers;
    internal IReadOnlyDictionary<IRoute, Func<TransformedWmRequest, Task<WmResponse>>> AsyncPostMessageHandlers => _asyncPostMessageHandlers;

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

    internal HandlerBuilderBase MapPost(IRoute route, Func<TransformedWmRequest, WmResponse> handler)
    {
        _postMessageHandlers.Add(route, handler);
        return this;
    }

    internal HandlerBuilderBase MapPost(IRoute route, Func<TransformedWmRequest, Task<WmResponse>> handler)
    {
        _asyncPostMessageHandlers.Add(route, handler);
        return this;
    }
}
