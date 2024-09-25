using System;

namespace DesktopMinimalAPI.Core.RequestHandling.Models;

public readonly record struct UrlParameterString(string Value)
{
    public UrlParameterString() : this(string.Empty) { } // Throws

    public string Value { get; } =
        !string.IsNullOrWhiteSpace(Value)
            ? Value
            : throw new ArgumentException("Value cannot be null or whitespace.", nameof(Value));

    public override string ToString() => Value;
    public static implicit operator string(UrlParameterString parameter) => parameter.ToString();
}
