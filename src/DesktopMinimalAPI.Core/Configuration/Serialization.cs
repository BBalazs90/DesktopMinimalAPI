using System.Text.Json;

namespace DesktopMinimalAPI.Core.Configuration;
public static class Serialization
{
    public static readonly JsonSerializerOptions DefaultCamelCase = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
}
