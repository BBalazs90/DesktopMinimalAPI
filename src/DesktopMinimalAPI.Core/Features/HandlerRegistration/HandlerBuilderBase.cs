using DesktopMinimalAPI.Core.Abstractions;
using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using DesktopMinimalAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopMinimalAPI.Core.Features.HandlerRegistration;

public abstract class HandlerBuilderBase
{
    private readonly Dictionary<Route, Func<WmRequest, Task<WmResponse>>> _getMessageHandlers = [];
    private readonly Dictionary<Route, Func<WmRequest, Task<WmResponse>>> _postMessageHandlers = [];

    internal IReadOnlyDictionary<Route, Func<WmRequest, Task<WmResponse>>> GetMessageHandlers => _getMessageHandlers;
    internal IReadOnlyDictionary<Route, Func<WmRequest, Task<WmResponse>>> PostMessageHandlers => _postMessageHandlers;

    public abstract Task<IWebMessageBroker> BuildAsync();

    internal HandlerBuilderBase MapGet(Route route, Func<WmRequest, WmResponse> handler)
    {
        _getMessageHandlers.Add(route, (req) => Task.FromResult(handler(req)));
        return this;
    }

    internal HandlerBuilderBase MapGet(Route route, Func<WmRequest, Task<WmResponse>> handler)
    {
        _getMessageHandlers.Add(route, handler);
        return this;
    }

    internal HandlerBuilderBase MapPost(Route route, Func<WmRequest, WmResponse> handler)
    {
        _postMessageHandlers.Add(route, (req) => Task.FromResult(handler(req)));
        return this;
    }

    internal HandlerBuilderBase MapPost(Route route, Func<WmRequest, Task<WmResponse>> handler)
    {
        _postMessageHandlers.Add(route, handler);
        return this;
    }
}
