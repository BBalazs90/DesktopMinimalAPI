using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using DesktopMinimalAPI.Models;
using System;
using System.Net;
using System.Threading.Tasks;
using static DesktopMinimalAPI.Core.HandlerRegistration.ResponseHelper;
using static DesktopMinimalAPI.Core.ParameterReading.ParameterReader;

namespace DesktopMinimalAPI.Core.HandlerRegistration.Async;
internal static class AsyncHandlerTransformer
{
    internal static Func<WmRequest, Task<WmResponse>> Transform<T>(Func<Task<HandlerResult<T>>> handler) =>
      (request) => handler().ContinueWith(task => CreateResponseFromHandlerResult(task, request.Id),
          TaskScheduler.Current);

    internal static Func<WmRequest, Task<WmResponse>> Transform<TIn, TOut>(Func<FromUrl<TIn>, Task<HandlerResult<TOut>>> handler) =>
    (request) => GetUrlParameter<TIn>(request.Route.Parameters)
                .Match(Some: parameter => handler(parameter)
                                        .ContinueWith(task => CreateResponseFromHandlerResult(task, request.Id),
                                   TaskScheduler.Current),
                       None: () => CreateInvalidUrlParameterResponse(request.Id));

    internal static Func<WmRequest, Task<WmResponse>> Transform<TIn1, TIn2, TOut>(Func<FromUrl<TIn1>, FromUrl<TIn2>, Task<HandlerResult<TOut>>> handler) =>
    (request) => GetUrlParameters<TIn1, TIn2>(request.Route.Parameters)
                .Match(Some: parameters => handler(parameters.Item1, parameters.Item2)
                                        .ContinueWith(task => CreateResponseFromHandlerResult(task, request.Id),
                                   TaskScheduler.Current),
                       None: () => CreateInvalidUrlParameterResponse(request.Id));

    internal static Func<WmRequest, Task<WmResponse>> Transform<TIn, TOut>(Func<FromBody<TIn>, Task<HandlerResult<TOut>>> handler) =>
     (request) => request.Body
                    .Bind<Task<WmResponse>>(body => GetBodyParameter<TIn>(body)
                                                    .Match(Some: parameter => handler(parameter)
                                                                            .ContinueWith(task => CreateResponseFromHandlerResult(task, request.Id),
                                                                          TaskScheduler.Current),
                                                           None: () => CreateInvalidBodyParameterResponse(request.Id)))
                    .IfNone(CreateInvalidBodyParameterResponse(request.Id));

    private static WmResponse CreateResponseFromHandlerResult<T>(Task<HandlerResult<T>> handlerTask, RequestId requestId) =>
        handlerTask.IsCompletedSuccessfully
        ? new WmResponse(requestId,
                    handlerTask.Result.StatusCode,
                    CreateResponseBody(handlerTask.Result))
        : new WmResponse(requestId,
                HttpStatusCode.BadRequest,
                CreateResponseBody(handlerTask.Exception?.InnerException
                    ?? new InvalidOperationException("Unknown exception in handler")));

    private static Task<WmResponse> CreateInvalidUrlParameterResponse(RequestId requestId) =>
        Task.FromResult(new WmResponse(requestId,
                                        HttpStatusCode.BadRequest,
                                        CreateResponseBody("The provided URL did not contained proper parameter.")));

    private static Task<WmResponse> CreateInvalidBodyParameterResponse(RequestId requestId) =>
        Task.FromResult(new WmResponse(requestId,
                                        HttpStatusCode.BadRequest,
                                        CreateResponseBody("The provided body did not contained proper parameter.")));
}
