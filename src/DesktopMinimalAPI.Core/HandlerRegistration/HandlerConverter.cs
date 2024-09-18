using LanguageExt.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DesktopMinimalAPI.Core.HandlerRegistration;
internal static class HandlerConverter
{
    public static Func<HandlerResult<T>> Transform<T>(Func<T> source) =>
        () => new HandlerResult<T>(source(), HttpStatusCode.OK);
}

public readonly record struct HandlerResult<T>(T Value, HttpStatusCode StatusCode);

public static class HandlerResult
{
    public static HandlerResult<T> Ok<T>(T value) => new(value, HttpStatusCode.OK);
    public static HandlerResult<T> BadRequest<T>(T value) => new(value, HttpStatusCode.BadRequest);
    public static HandlerResult<T> InternalServerError<T>(T value) => new(value, HttpStatusCode.InternalServerError);
}
