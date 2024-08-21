using LanguageExt;
using System.Globalization;

namespace DesktopMinimalAPI.Core.RequestHandling.Models.Methods;

public abstract class Method
{
    public static readonly Get Get = new();
    public static readonly Post Post = new();

    public static Option<Method> Parse(string? method) =>
        method?.ToUpper(CultureInfo.CurrentCulture) switch
        {
            "GET" => Get,
            "POST" => Post,
            _ => Option<Method>.None
        };
}
