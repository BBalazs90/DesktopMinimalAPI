using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopMinimalAPI.Models;

public record BxiosRequest(string RequestId, string Method, string Path, string? Body = null);