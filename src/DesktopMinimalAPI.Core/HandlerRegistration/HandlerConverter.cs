﻿using LanguageExt;
using System.Net;

namespace DesktopMinimalAPI.Core.HandlerRegistration;

public readonly record struct Message(string Value)
{
    public Message() : this(string.Empty) { }

    public static implicit operator Message(string value) => new(value);

    public static implicit operator string(Message wrappee) => wrappee.Value;

}
public readonly record struct HandlerResult<T>(Either<Message, T> Value, HttpStatusCode StatusCode);

public static class HandlerResult
{
    public static HandlerResult<bool> Ok() => new(Either<Message,bool>.Left(string.Empty), HttpStatusCode.OK);
    public static HandlerResult<T> Ok<T>() => new(Either<Message,T>.Left(string.Empty), HttpStatusCode.OK);
    public static HandlerResult<T> Ok<T>(T value) => new(value, HttpStatusCode.OK);
    public static HandlerResult<T> BadRequest<T>() => new(Either<Message, T>.Left(string.Empty), HttpStatusCode.BadRequest);
    public static HandlerResult<T> BadRequest<T>(Message msg) => new(Either<Message, T>.Left(msg), HttpStatusCode.BadRequest);
    public static HandlerResult<T> BadRequest<T>(T value) => new(value, HttpStatusCode.BadRequest);
    public static HandlerResult<T> InternalServerError<T>() => new(Either<Message, T>.Left(string.Empty), HttpStatusCode.InternalServerError);
    public static HandlerResult<T> InternalServerError<T>(Message msg) => new(Either<Message, T>.Left(msg), HttpStatusCode.InternalServerError);
    public static HandlerResult<T> InternalServerError<T>(T value) => new(value, HttpStatusCode.InternalServerError);
    public static HandlerResult<T> Unauthorized<T>() => new(Either<Message, T>.Left(string.Empty), HttpStatusCode.Unauthorized);
    public static HandlerResult<T> Unauthorized<T>(Message msg) => new(Either<Message, T>.Left(msg), HttpStatusCode.Unauthorized);
    public static HandlerResult<T> Unauthorized<T>(T value) => new(value, HttpStatusCode.Unauthorized);
}
