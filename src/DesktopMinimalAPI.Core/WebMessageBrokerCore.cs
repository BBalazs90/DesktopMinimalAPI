using DesktopMinimalAPI.Core;
using DesktopMinimalAPI.Core.Abstractions;
using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.Models;
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

    internal required ImmutableDictionary<IRoute, Func<TransformedWmRequest, WmResponse>> GetMessageHandlers { get; init; }
    internal required ImmutableDictionary<IRoute, Func<TransformedWmRequest, Task<WmResponse>>> AsyncGetMessageHandlers { get; init; }

    internal void OnWebMessageReceived(object? sender, EventArgs e) => StartRequestProcessingPipeline(e);

    private void StartRequestProcessingPipeline(EventArgs e)
    {
        var (request, invalidRequestReponse) = TryGetRequest(e);
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
            _ => null
        };

        if (handler is null)
        {
            var notFoundResponse = new WmResponse(request.RequestId, HttpStatusCode.NotFound, JsonSerializer.Serialize($"The requested endpoint '{request.Path}' was not registered", Serialization.DefaultCamelCase));
            CoreWebView?.PostWebMessageAsString(JsonSerializer.Serialize(notFoundResponse, Serialization.DefaultCamelCase));
            return;
        }

        //if (handler is null)
        //{
        //    var asyncHandler = request.Method switch
        //    {
        //        var method when method == Methods.Get => AsyncGetMessageHandlers.GetValueOrDefault((StringRoute)(request.Path)),
        //        _ => null
        //    };

        //    Task.Run(async () => await asyncHandler?
        //        .Invoke(request)
        //        .ContinueWith(resp =>
        //        {
        //            _context.Post(_ =>
        //            {
        //                CoreWebView?.PostWebMessageAsString(JsonSerializer.Serialize(resp.Result, Serialization.DefaultCamelCase));
        //            }, null);

        //        }));

        //    return;
        //}

        var response = handler.Invoke(transformedRequest);

        CoreWebView?.PostWebMessageAsString(JsonSerializer.Serialize(response, Serialization.DefaultCamelCase));


        static (WmRequest? retrievedRequest, WmResponse? invalidRequestReponse) TryGetRequest(EventArgs e)
        {
#pragma warning disable CA1031 // Do not catch general exception types
            // This must catch all exception, because that must be returned to the caller.
            try
            {
                var request = JsonSerializer.Deserialize<WmRequest>(GetWebMessageAsString(e), Serialization.DefaultCamelCase);
                return request switch
                {
                    null => (null, new WmResponse(Guid.Empty, HttpStatusCode.BadRequest, "The request was not properly formated")),
                    (var id, _, _, _) when id == Guid.Empty => (null, new WmResponse(Guid.Empty, HttpStatusCode.BadRequest, "The request must contain a valid GUID")),
                    (var id, var method, _, _) when method is null || method == Method.Invalid => (null, new WmResponse(id, HttpStatusCode.BadRequest, "The request must contain a valid request method type (GET | POST | PUT | DELETE")),
                    (var id, _, var path, _) when string.IsNullOrWhiteSpace(path) => (null, new WmResponse(id, HttpStatusCode.BadRequest, "The request must contain a valid, non-empty path")),
                    _ => (request, null)
                };
            }
            catch (Exception ex)
            {
                return (null, new WmResponse(Guid.Empty, HttpStatusCode.BadRequest, ex.Message));
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        static string GetWebMessageAsString(EventArgs e) =>
            // This required for testing purposes, since CoreWebView2WebMessageReceivedEventArgs has no public ctr, thus not possible to simulate
            // the even fire. Therefore reflection magic is needed, but this overhead is eliminated in prod.
#if DEBUG
            e is CoreWebView2WebMessageReceivedEventArgs cwvArg
            ? cwvArg.WebMessageAsJson
            : (e.GetType().GetProperty("WebMessageAsJson")?.GetValue(e) as string ?? throw new ArgumentException("The provided type has no WebMessageAsJson property"));
#else
        ((CoreWebView2WebMessageReceivedEventArgs)e).WebMessageAsJson;
#endif

        void PostResponse(WmResponse response) =>
            CoreWebView.PostWebMessageAsString(JsonSerializer.Serialize(response, Serialization.DefaultCamelCase));
    }
}

