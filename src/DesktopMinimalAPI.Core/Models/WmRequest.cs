using DesktopMinimalAPI.Core.Models.Dtos;
using DesktopMinimalAPI.Core.Models.Methods;
using System;

namespace DesktopMinimalAPI.Models;

internal static class WmRequestBuilder
{
    public static WmRequest BuildFrom(WmRequestDto? requestDto)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        _ = Guid.TryParse(requestDto.RequestId, out var requestId)
            && requestId != Guid.Empty
            ? requestId
            : throw new ArgumentException(nameof(requestDto.RequestId));
        var method = (Method)requestDto.Method;
        var path = !string.IsNullOrWhiteSpace(requestDto.Path) ? requestDto.Path : throw new ArgumentNullException(nameof(requestDto));

        return new WmRequest(requestId, method, path, requestDto.Body);
    }
}

public record WmRequest(Guid RequestId, Method Method, string Path, string? Body = null);

