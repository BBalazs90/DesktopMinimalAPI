using DesktopMinimalAPI.Core.Models.Dtos;
using DesktopMinimalAPI.Core.Models.Methods;
using LanguageExt;
using LanguageExt.Pipes;
using System;
using System.IO;

namespace DesktopMinimalAPI.Models;

internal static class WmRequestBuilder
{
    public static WmRequestType BuildFrom(WmRequestDto? requestDto)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        _ = Guid.TryParse(requestDto.RequestId, out var requestId)
            && requestId != Guid.Empty
            ? requestId
            : throw new ArgumentException(nameof(requestDto.RequestId));
        var method = (Method)requestDto.Method;
        var path = !string.IsNullOrWhiteSpace(requestDto.Path) ? requestDto.Path : throw new ArgumentNullException(nameof(requestDto));

        return new WmRequestType(requestId, method, path, requestDto.Body);
    }
}

public record WmRequestType(Guid RequestId, Method Method, string Path, string? Body = null);
internal static class WmRequest
{
    public static Either<Exception, WmRequestType> From(WmRequestDto dto) =>  new WmRequestType(Guid.Parse(dto.RequestId), (Method)dto.Method, dto.Path);
}

