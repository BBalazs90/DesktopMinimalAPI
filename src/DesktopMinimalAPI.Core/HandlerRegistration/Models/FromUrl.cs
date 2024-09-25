using System;
using System.Diagnostics.CodeAnalysis;

namespace DesktopMinimalAPI.Core.HandlerRegistration.Models;

public readonly record struct FromUrl<T>(T Value)
{
    public FromUrl() : this(default!) /*Not allowed, so safe to bang*/ =>
        throw new InvalidOperationException($"{nameof(FromUrl<T>)} is not allowed to create without a wrapped value!");

    [SuppressMessage("Design", "CA2225: Operator overloads have named alternates",
        Justification = "This is a lighweight wrapper record struct, only intented to use" +
        "as a marker, and get its content implicitly.")]
    public static implicit operator T(FromUrl<T> wrapper) => wrapper.Value;

    [SuppressMessage("Design", "CA2225: Operator overloads have named alternates",
        Justification = "This is a lighweight wrapper record struct, only intented to use" +
        "as a marker, and get its content implicitly.")]
    public static implicit operator FromUrl<T>(T wrappee) => new(wrappee);
}
