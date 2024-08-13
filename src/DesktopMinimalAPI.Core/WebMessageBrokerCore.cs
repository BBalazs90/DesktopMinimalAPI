using DesktopMinimalAPI.Core;
using DesktopMinimalAPI.Core.Abstractions;
using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Core.Models.Methods;
using DesktopMinimalAPI.Core.RequestHandling;
using DesktopMinimalAPI.Models;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("DesktopMinimalAPI.Core.Tests")]
[assembly: InternalsVisibleTo("DesktopMinimalAPI.WPF")]
namespace DesktopMinimalAPI;

internal sealed class WebMessageBrokerCore : IWebMessageBroker
{
    public readonly ICoreWebView2 CoreWebView;
#pragma warning disable S4487 // Unread "private" fields should be removed
    // Will be used soon.
    private readonly SynchronizationContext? _context;
#pragma warning restore S4487 // Unread "private" fields should be removed

    internal WebMessageBrokerCore(ICoreWebView2 coreWebView)
    {
        CoreWebView = coreWebView;
        CoreWebView.WebMessageReceived += OnWebMessageReceived;
        _context = SynchronizationContext.Current;
    }

    internal required ImmutableDictionary<IRoute, Func<TransformedWmRequest, Task<WmResponse>>> GetMessageHandlers { get; init; }
    internal required ImmutableDictionary<IRoute, Func<TransformedWmRequest, Task<WmResponse>>> PostMessageHandlers { get; init; }

    internal void OnWebMessageReceived(object? sender, EventArgs e) => StartRequestProcessingPipeline(e);

    private void StartRequestProcessingPipeline(EventArgs e)
    {
        var (request, invalidRequestReponse) = RequestReaderPipeline.TryGetRequest(e);
        if (invalidRequestReponse is not null)
        {
            PostResponse(invalidRequestReponse);
            return;
        }

        Debug.Assert(request is not null, "If the invalidRequestReponse is not null, the previous method must return a non-null request");
        var route = RoutePipeline.GetRoot(request.Path);
        var transformedRequest = RequestTransformerPipeline.Transform(request);
        var handler = request.Method switch
        {
            var method when method == Method.Get => GetMessageHandlers.GetValueOrDefault(route),
            var method when method == Method.Post => PostMessageHandlers.GetValueOrDefault(route),
            _ => null
        };

        if (handler is null)
        {
            var notFoundResponse = new WmResponse(request.RequestId, HttpStatusCode.NotFound, JsonSerializer.Serialize($"The requested endpoint '{request.Path}' was not registered", Serialization.DefaultCamelCase));
            CoreWebView?.PostWebMessageAsString(JsonSerializer.Serialize(notFoundResponse, Serialization.DefaultCamelCase));
            return;
        }

        _ = Task.Run(async () => await handler
            .Invoke(transformedRequest)
            .ContinueWith(resp => 
                _context?.Post(
                    _ => CoreWebView?.PostWebMessageAsString(JsonSerializer.Serialize(resp.Result, Serialization.DefaultCamelCase)), null),
                    TaskScheduler.Current)
            .ConfigureAwait(false));



        void PostResponse(WmResponse response) =>
            CoreWebView.PostWebMessageAsString(JsonSerializer.Serialize(response, Serialization.DefaultCamelCase));

    }
}

