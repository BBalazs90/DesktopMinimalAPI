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

    internal void OnWebMessageReceived(object? sender, EventArgs e)
    {
        var request = JsonSerializer.Deserialize<WmRequest>(GetWebMessageAsString(e), Serialization.DefaultCamelCase);
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

    private static string GetWebMessageAsString(EventArgs e) =>
        // This required for testing purposes, since CoreWebView2WebMessageReceivedEventArgs has no public ctr, thus not possible to simulate
        // the even fire. Therefore reflection magic is needed, but this overhead is eliminated in prod.
#if DEBUG
        e is CoreWebView2WebMessageReceivedEventArgs cwvArg ? cwvArg.WebMessageAsJson : (string)e.GetType().GetProperty("WebMessageAsJson").GetValue(e);
#else
        ((CoreWebView2WebMessageReceivedEventArgs)e).WebMessageAsJson;
#endif
}

