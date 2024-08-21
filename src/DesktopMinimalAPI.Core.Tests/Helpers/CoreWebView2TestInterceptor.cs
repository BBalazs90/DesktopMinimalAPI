using DesktopMinimalAPI.Core.Abstractions;
using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using DesktopMinimalAPI.Core.RequestHandling.Models.Dtos;
using DesktopMinimalAPI.Core.RequestHandling.Models.Methods;
using DesktopMinimalAPI.Models;
using LanguageExt.UnsafeValueAccess;
using System.Text.Json;

namespace DesktopMinimalAPI.Core.Tests.Helpers;

internal class CoreWebView2TestInterceptor : ICoreWebView2
{

    private readonly TaskCompletionSource<bool> _tcs = new();
    private readonly CancellationTokenSource _cts = new(TimeSpan.FromSeconds(2));
    public event EventHandler<EventArgs>? WebMessageReceived;

    public CoreWebView2TestInterceptor()
    {
        _ = _cts.Token.Register(() => _tcs.SetException(new Exception("Failed to recive response in time")));
    }

    public string LastPostedWebMessageAsString { get; private set; } = string.Empty;

    public void PostWebMessageAsString(string webMessageAsString)
    {
        LastPostedWebMessageAsString = webMessageAsString;
        _ = _tcs.TrySetResult(true);
        _cts.Dispose();
    }

    public void RaiseWebMessageReceived(string message)
    {
        var arg = new MockCoreWebView2WebMessageReceivedEventArgs(message);
        WebMessageReceived?.Invoke(this, arg);
    }

    public Task<bool> WaitUntilResponseReceivedAsync() => _tcs.Task;
}

internal static class CoreWebView2TestInterceptorExtensions
{
    public static Guid SimulateGet(this CoreWebView2TestInterceptor webView, string path, string? body = null)
    {
        var requestId = Guid.NewGuid();
        webView.RaiseWebMessageReceived(BuildSerializedRequest(requestId, Method.Get, path, body));
        return requestId;
    }

    public static RequestId SimulatePost(this CoreWebView2TestInterceptor webView, string path)
    {
        var requestId = RequestId.From(Guid.NewGuid().ToString()).ValueUnsafe();
        webView.RaiseWebMessageReceived(BuildSerializedRequest(requestId, Method.Post, path));
        return requestId;
    }

    public static WmResponse ReadLastResponse(this CoreWebView2TestInterceptor webView) =>
        JsonSerializer.Deserialize<WmResponse>(webView.LastPostedWebMessageAsString, Serialization.DefaultCamelCase)
        ?? throw new InvalidOperationException($"Could not deserialize the last webmessage. Its content: '{webView.LastPostedWebMessageAsString}'");

    public static async Task<WmResponse> ReadLastResponseAsync(this CoreWebView2TestInterceptor webView)
    {
        _ = await webView.WaitUntilResponseReceivedAsync();
        return JsonSerializer.Deserialize<WmResponse>(webView.LastPostedWebMessageAsString, Serialization.DefaultCamelCase)
        ?? throw new InvalidOperationException($"Could not deserialize the last webmessage. Its content: '{webView.LastPostedWebMessageAsString}'");
    }

    private static string BuildSerializedRequest(Guid requestId, Method method, string path, string? body = null) =>
        JsonSerializer.Serialize(new WmRequestDto()
        {
            RequestId = requestId.ToString(),
            Method = method.ToString(),
            Path = path,
            Body = body
        },
        Serialization.DefaultCamelCase);
}
