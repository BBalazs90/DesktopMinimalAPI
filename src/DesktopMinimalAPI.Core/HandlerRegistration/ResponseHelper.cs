using DesktopMinimalAPI.Core.RequestHandling.Models;
using DesktopMinimalAPI.Models;
using System;
using System.Net;
using System.Threading.Tasks;
using static DesktopMinimalAPI.Core.Support.SerializationHelper;

namespace DesktopMinimalAPI.Core.HandlerRegistration;
internal static class ResponseHelper
{
    internal static string CreateResponseBody<T>(HandlerResult<T> result) =>
      result.Value.Match<string>(
                     Left: msg => SerializeCamelCase(new { Message = (string)msg }),
                     Right: value => SerializeCamelCase(value));

    internal static string CreateResponseBody(Exception ex) =>
        SerializeCamelCase(new { Message = ex.Message });

    internal static string CreateResponseBody(string message) =>
        SerializeCamelCase(new { Message = message });

    internal static WmResponse CreateResponseFromHandlerResult<T>(Task<HandlerResult<T>> handlerTask, RequestId requestId) =>
       handlerTask.IsCompletedSuccessfully
       ? new WmResponse(requestId,
                   handlerTask.Result.StatusCode,
                   CreateResponseBody(handlerTask.Result))
       : new WmResponse(requestId,
               HttpStatusCode.BadRequest,
               CreateResponseBody(handlerTask.Exception?.InnerException
                   ?? new InvalidOperationException("Unknown exception in handler")));

    internal static Task<WmResponse> CreateInvalidUrlParameterResponse(RequestId requestId) =>
        Task.FromResult(new WmResponse(requestId,
                                        HttpStatusCode.BadRequest,
                                        CreateResponseBody("The provided URL did not contained proper parameter.")));

    internal static Task<WmResponse> CreateInvalidBodyParameterResponse(RequestId requestId) =>
        Task.FromResult(new WmResponse(requestId,
                                        HttpStatusCode.BadRequest,
                                        CreateResponseBody("The provided body did not contained proper parameter.")));
}
