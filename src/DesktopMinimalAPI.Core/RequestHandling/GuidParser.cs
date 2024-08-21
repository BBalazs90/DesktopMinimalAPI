using LanguageExt;
using System;

namespace DesktopMinimalAPI.Core.RequestHandling;
internal static class GuidParser
{
    public static Option<Guid> Parse(string? guid) =>
        Guid.TryParse(guid, out var parsedGuid) && parsedGuid != Guid.Empty
            ? parsedGuid
            : Option<Guid>.None;
}
