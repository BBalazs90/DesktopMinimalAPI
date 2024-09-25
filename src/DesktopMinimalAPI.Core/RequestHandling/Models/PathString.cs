using System;

namespace DesktopMinimalAPI.Core.RequestHandling.Models;

public readonly record struct PathString(string Value)
{
    public PathString() : this(string.Empty) { } // Throws

    public string Value { get; } =
        !string.IsNullOrWhiteSpace(Value) && Value.StartsWith('/')
            ? Value
            : throw new ArgumentException("Value cannot be null or whitespace, and must start with '/'.", nameof(Value));

    public override string ToString() => Value;
    public static implicit operator string(PathString rootString) => rootString.ToString();
}
