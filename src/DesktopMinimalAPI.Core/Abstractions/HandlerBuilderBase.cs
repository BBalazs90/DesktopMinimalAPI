using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopMinimalAPI.Core.Abstractions;

public abstract class HandlerBuilderBase
{
    private readonly Dictionary<IRoute, Func<TransformedWmRequest, Task<WmResponse>>> _getMessageHandlers = [];
    private readonly Dictionary<IRoute, Func<TransformedWmRequest, Task<WmResponse>>> _postMessageHandlers = [];

    internal IReadOnlyDictionary<IRoute, Func<TransformedWmRequest, Task<WmResponse>>> GetMessageHandlers => _getMessageHandlers;
    internal IReadOnlyDictionary<IRoute, Func<TransformedWmRequest, Task<WmResponse>>> PostMessageHandlers => _postMessageHandlers;

    public abstract Task<IWebMessageBroker> BuildAsync();

    internal HandlerBuilderBase MapGet(IRoute route, Func<TransformedWmRequest, WmResponse> handler)
    {
        _getMessageHandlers.Add(route, (req) => Task.FromResult(handler(req)));
        return this;
    }

    internal HandlerBuilderBase MapGet(IRoute route, Func<TransformedWmRequest, Task<WmResponse>> handler)
    {
        _getMessageHandlers.Add(route, handler);
        return this;
    }

    internal HandlerBuilderBase MapPost(IRoute route, Func<TransformedWmRequest, WmResponse> handler)
    {
        _postMessageHandlers.Add(route, (req) => Task.FromResult(handler(req)));
        return this;
    }

    internal HandlerBuilderBase MapPost(IRoute route, Func<TransformedWmRequest, Task<WmResponse>> handler)
    {
        _postMessageHandlers.Add(route, handler);
        return this;
    }
}
