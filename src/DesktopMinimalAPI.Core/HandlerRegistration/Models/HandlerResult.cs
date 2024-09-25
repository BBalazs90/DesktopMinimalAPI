using LanguageExt;
using System.Net;

namespace DesktopMinimalAPI.Core.HandlerRegistration.Models;

public static class HandlerResult
{
    private static readonly object _empty = new();

    public static HandlerResult<object> Ok() => new(_empty, HttpStatusCode.OK);
    public static HandlerResult<T> Ok<T>() => new(Either<Message, T>.Left(string.Empty), HttpStatusCode.OK);
    public static HandlerResult<T> Ok<T>(T value) => new(value, HttpStatusCode.OK);
    public static HandlerResult<T> BadRequest<T>() => new(Either<Message, T>.Left(string.Empty), HttpStatusCode.BadRequest);
    public static HandlerResult<T> BadRequest<T>(Message msg) => new(Either<Message, T>.Left(msg), HttpStatusCode.BadRequest);
    public static HandlerResult<T> InternalServerError<T>() => new(Either<Message, T>.Left(string.Empty), HttpStatusCode.InternalServerError);
    public static HandlerResult<T> InternalServerError<T>(Message msg) => new(Either<Message, T>.Left(msg), HttpStatusCode.InternalServerError);
    public static HandlerResult<T> Unauthorized<T>() => new(Either<Message, T>.Left(string.Empty), HttpStatusCode.Unauthorized);
    public static HandlerResult<T> Unauthorized<T>(Message msg) => new(Either<Message, T>.Left(msg), HttpStatusCode.Unauthorized);
    public static HandlerResult<T> Unauthorized<T>(T value) => new(value, HttpStatusCode.Unauthorized);
}
public readonly record struct HandlerResult<T>(Either<Message, T> Value, HttpStatusCode StatusCode);
