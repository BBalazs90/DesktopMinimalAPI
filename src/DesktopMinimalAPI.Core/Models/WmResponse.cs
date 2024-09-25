using System;
using System.Net;

namespace DesktopMinimalAPI.Models;

public record WmResponse(Guid RequestId, HttpStatusCode Status, string Data);
