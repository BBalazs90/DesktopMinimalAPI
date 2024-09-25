using DesktopMinimalAPI.Core.Abstractions;
using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using DesktopMinimalAPI.Core.RequestHandling.Models.Exceptions;
using DesktopMinimalAPI.Core.RequestHandling.Models.Methods;
using DesktopMinimalAPI.Models;
using LanguageExt;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static DesktopMinimalAPI.Core.RequestHandling.RequestDecoder;

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

    internal void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e) => StartRequestProcessingPipeline(e);

    private void StartRequestProcessingPipeline(CoreWebView2WebMessageReceivedEventArgs e)
    {
       var request = DecodeRequest(e);
        _ = Task.Run(() =>
            request
            .Bind(FindHandler)
            .Map(async handler => await handler().ConfigureAwait(false))
            .Match(
                Right: response => response.ContinueWith(r => PostResponse(r.Result, _context), TaskScheduler.Default),
                Left: ex => ex switch
                {
                    RequestException reqEx when reqEx.InnerException is KeyNotFoundException => HandleException(new WmResponse(reqEx.RequestId, HttpStatusCode.NotFound, string.Join(' ', reqEx.Message, reqEx.InnerException?.Message))),
                    RequestException reqEx => HandleException(new WmResponse(reqEx.RequestId, HttpStatusCode.BadRequest, string.Join(' ', reqEx.Message, reqEx.InnerException?.Message))),
                    _ => HandleException(new WmResponse(Guid.Empty, HttpStatusCode.InternalServerError, ex.Message))
                }
              ));

    }

    void PostResponse(WmResponse response, SynchronizationContext context) =>
        context.Post((state) =>
            CoreWebView.PostWebMessageAsString(JsonSerializer.Serialize(state, Serialization.DefaultCamelCase)), response);

    Task HandleException(WmResponse response)
    {
        PostResponse(response, _context);
        return Task.CompletedTask;
    }

    [SuppressMessage("Usage", "CA2201: Exception is not specific enough",
    Justification = "That exception is not used, only needed because pattern matching in C# requires a 'catch all' case.")]
    private Either<RequestException, Func<Task<WmResponse>>> FindHandler(WmRequest request) => request.Method switch
    {
        Get => GetMessageHandlers.TryGetValue(request.Route, out var handler)
            ? new Func<Task<WmResponse>>(() => handler(request))
            : RequestException.From(request.Id, new KeyNotFoundException($"No handler was registered for the route '{request.Route.Path}'")),
        Post => PostMessageHandlers.TryGetValue(request.Route, out var handler)
           ? new Func<Task<WmResponse>>(() => handler(request))
           : RequestException.From(request.Id, new KeyNotFoundException($"No handler was registered for the route '{request.Route.Path}'")),
        _ => RequestException.From(request.Id, new Exception("This cannot happen"))
    };
}

