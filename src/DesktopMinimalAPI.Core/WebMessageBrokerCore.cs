using DesktopMinimalAPI.Core.Abstractions;
using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Models;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("DesktopMinimalAPI.Core.Tests")]
namespace DesktopMinimalAPI;

public sealed partial class WebMessageBrokerCore
{
    public readonly ICoreWebView2 CoreWebView;
    private readonly SynchronizationContext? _context;

    public WebMessageBrokerCore(ICoreWebView2 coreWebView)
    {
        CoreWebView = coreWebView;
        CoreWebView.WebMessageReceived += OnWebMessageReceived;
        _context = SynchronizationContext.Current;
    }

    public required ImmutableDictionary<IRoute, Func<WmRequest, WmResponse>> GetMessageHandlers { get; init; }
    public required ImmutableDictionary<IRoute, Func<WmRequest, Task<WmResponse>>> AsyncGetMessageHandlers { get; init; }

    internal void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        var request = JsonSerializer.Deserialize<WmRequest>(e.WebMessageAsJson, Serialization.DefaultCamelCase);
        if (request is null) return;

        var handler = request.Method switch
        {
            Methods.Get => GetMessageHandlers.GetValueOrDefault((StringRoute)(request.Path)),
            _ => null
        };

        if (handler is null)
        {
            var asyncHandler = request.Method switch
            {
                Methods.Get => AsyncGetMessageHandlers.GetValueOrDefault((StringRoute)(request.Path)),
                _ => null
            };

            Task.Run(async () => await asyncHandler?
                .Invoke(request)
                .ContinueWith(resp =>
            {
                _context.Post(_ =>
                {
                    CoreWebView?.PostWebMessageAsString(JsonSerializer.Serialize(resp.Result, Serialization.DefaultCamelCase));
                }, null);

            }));

            return;
        }

        var response = handler?.Invoke(request);


        CoreWebView?.PostWebMessageAsString(JsonSerializer.Serialize(response, Serialization.DefaultCamelCase));
    }
}

