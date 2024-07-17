namespace DesktopMinimalAPI.Models;

public record WmRequest(string RequestId, Methods Method, string Path, string? Body = null);