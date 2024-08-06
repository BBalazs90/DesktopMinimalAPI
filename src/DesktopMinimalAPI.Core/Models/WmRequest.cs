using DesktopMinimalAPI.Core.Models.Methods;
using System;

namespace DesktopMinimalAPI.Models;

public record WmRequest(Guid RequestId, Method Method, string Path, string? Body = null);