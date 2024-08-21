using DesktopMinimalAPI.Core.RequestHandling;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using DesktopMinimalAPI.Core.RequestHandling.Models.Dtos;
using DesktopMinimalAPI.Core.RequestHandling.Models.Exceptions;
using DesktopMinimalAPI.Core.RequestHandling.Models.Methods;
using LanguageExt;
using LanguageExt.Pipes;
using System;
using System.IO;

namespace DesktopMinimalAPI.Models;

internal static class WmRequestBuilder
{
    //public static WmRequestType BuildFrom(WmRequestDto? requestDto)
    //{
    //    ArgumentNullException.ThrowIfNull(requestDto);

    //    _ = Guid.TryParse(requestDto.RequestId, out var requestId)
    //        && requestId != Guid.Empty
    //        ? requestId
    //        : throw new ArgumentException(nameof(requestDto.RequestId));
    //    var method = (Method)requestDto.Method;
    //    var path = !string.IsNullOrWhiteSpace(requestDto.Path) ? requestDto.Path : throw new ArgumentNullException(nameof(requestDto));

    //    return new WmRequestType(requestId, method, path, requestDto.Body);
    //}
}

public record WmRequestType(Guid Id, Method Method, Route Route);

internal static class WmRequest
{
    public static Either<RequestException, WmRequestType> From(WmRequestDto dto) =>
        GuidParser.Parse(dto.RequestId).ToEither(RequestException.From(new ArgumentException(nameof(dto.RequestId))))
        .Bind(guid => With(guid, dto));

    static Either<RequestException, WmRequestType> With(Guid requestId, WmRequestDto dto) =>
         from method in Method.Parse(dto.Method).ToEither(RequestWithValidGuidException.From(requestId, new ArgumentException(nameof(dto.Method))) as RequestException)
         from route in Route.From(dto.Path).ToEither(RequestWithValidGuidException.From(requestId, new ArgumentException(nameof(dto.Path))) as RequestException)
         select new WmRequestType(requestId, method, route);
}
