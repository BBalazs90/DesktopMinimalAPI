using DesktopMinimalAPI.Core.Configuration;
using System.Text.Json;

namespace DesktopMinimalAPI.Core.Support;
internal static class SerializationHelper
{
    internal static string SerializeCamelCase<T>(T value) =>
       JsonSerializer.Serialize(value, Serialization.DefaultCamelCase);
}
