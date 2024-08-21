using DesktopMinimalAPI.Core.RequestHandling;
using DesktopMinimalAPI.Core.RequestHandling.Models.Dtos;
using DesktopMinimalAPI.Core.RequestHandling.Models.Exceptions;
using DesktopMinimalAPI.Core.RequestHandling.Models.Methods;
using LanguageExt;
using LanguageExt.Pipes;
using System;

namespace DesktopMinimalAPI.Core.RequestHandling.Models;

public record WmRequest(RequestId Id, Method Method, Route Route)
{
    internal static Either<RequestException, WmRequest> From(WmRequestDto dto) =>
       RequestId.From(dto.RequestId)
        .ToEither(RequestException.From(new ArgumentException(nameof(dto.RequestId))))
       .Bind(guid => WithId(guid, dto));

    static Either<RequestException, WmRequest> WithId(RequestId requestId, WmRequestDto dto) =>
         from method in Method.Parse(dto.Method).ToEither(RequestException.From(requestId, new ArgumentException(nameof(dto.Method))))
         from route in Route.From(dto.Path).ToEither(RequestException.From(requestId, new ArgumentException(nameof(dto.Path))))
         select new WmRequest(requestId, method, route);
}
