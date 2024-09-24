using System;
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
}
