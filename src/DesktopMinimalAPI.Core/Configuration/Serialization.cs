using System.Text.Json;

namespace DesktopMinimalAPI.Core.Configuration;
internal static class Serialization
{
    internal static readonly JsonSerializerOptions DefaultCamelCase = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
}
