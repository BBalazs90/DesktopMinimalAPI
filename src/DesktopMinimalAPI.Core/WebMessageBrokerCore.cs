using DesktopMinimalAPI.Core.Abstractions;
using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using DesktopMinimalAPI.Core.RequestHandling.Models.Exceptions;
using DesktopMinimalAPI.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static DesktopMinimalAPI.Core.RequestHandling.RequestReaderPipeline;

[assembly: InternalsVisibleTo("DesktopMinimalAPI.Core.Tests")]
[assembly: InternalsVisibleTo("DesktopMinimalAPI.WPF")]
namespace DesktopMinimalAPI;

internal sealed class WebMessageBrokerCore : IWebMessageBroker
{
    public readonly ICoreWebView2 CoreWebView;
    private readonly SynchronizationContext? _context;

    internal WebMessageBrokerCore(ICoreWebView2 coreWebView)
    {
        CoreWebView = coreWebView;
        CoreWebView.WebMessageReceived += OnWebMessageReceived;
        _context = SynchronizationContext.Current;
    }

    internal required ImmutableDictionary<Route, Func<WmRequest, Task<WmResponse>>> GetMessageHandlers { get; init; }
    internal required ImmutableDictionary<Route, Func<WmRequest, Task<WmResponse>>> PostMessageHandlers { get; init; }

    internal void OnWebMessageReceived(object? sender, EventArgs e) => StartRequestProcessingPipeline(e);

    private void StartRequestProcessingPipeline(EventArgs e)
    {
        var reponse = DecodeRequest(e)
            .Bind(FindHandler)
            .Map(SafeInvokeHandler)
            .Match(
                Right: response => response,
                Left: ex => ex switch
                {
                    RequestException reqEx when reqEx.InnerException is KeyNotFoundException => new WmResponse(reqEx.RequestId, HttpStatusCode.NotFound, JsonSerializer.Serialize(reqEx)),
                    RequestException reqEx => new WmResponse(reqEx.RequestId, HttpStatusCode.BadRequest, JsonSerializer.Serialize(reqEx)),
                    _ => new WmResponse(Guid.Empty, HttpStatusCode.InternalServerError, ex.Message)
                }
                );


        PostResponse(reponse);

        //var (request, invalidRequestReponse) = RequestReaderPipeline.TryGetRequest(e);
        //if (invalidRequestReponse is not null)
        //{
        //    PostResponse(invalidRequestReponse);
        //    return;
        //}

        //Debug.Assert(request is not null, "If the invalidRequestReponse is not null, the previous method must return a non-null request");
        //var route = RoutePipeline.GetRoot(request.Path);
        //var transformedRequest = RequestTransformerPipeline.Transform(request);
        //var handler = request.Method switch
        //{
        //    var method when method == Method.Get => GetMessageHandlers.GetValueOrDefault(route),
        //    var method when method == Method.Post => PostMessageHandlers.GetValueOrDefault(route),
        //    _ => null
        //};

        //if (handler is null)
        //{
        //    var notFoundResponse = new WmResponse(request.RequestId, HttpStatusCode.NotFound, JsonSerializer.Serialize($"The requested endpoint '{request.Path}' was not registered", Serialization.DefaultCamelCase));
        //    CoreWebView?.PostWebMessageAsString(JsonSerializer.Serialize(notFoundResponse, Serialization.DefaultCamelCase));
        //    return;
        //}

        //_ = Task.Run(async () => await handler
        //    .Invoke(transformedRequest)
        //    .ContinueWith(resp => 
        //        _context?.Post(
        //            _ => CoreWebView?.PostWebMessageAsString(JsonSerializer.Serialize(resp.Result, Serialization.DefaultCamelCase)), null),
        //            TaskScheduler.Current)
        //    .ConfigureAwait(false));



        void PostResponse(WmResponse response) =>
            CoreWebView.PostWebMessageAsString(JsonSerializer.Serialize(response, Serialization.DefaultCamelCase));

    }

    private Either<RequestException, Task<WmResponse>> FindHandler(WmRequest request) =>
        GetMessageHandlers.TryGetValue(request.Route, out var handler)
        ? handler(request)
        : RequestException.From(request.Id, new KeyNotFoundException($"No handler was registered for the route '{request.Route.Path}'"));

    private WmResponse SafeInvokeHandler(Task<WmResponse> handler) => handler.Result;
}

