using System;

namespace DesktopMinimalAPI.Models;

public record WmRequest(Guid RequestId, Methods Method, string Path, string? Body = null);