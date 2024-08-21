using LanguageExt;
using System;

namespace DesktopMinimalAPI.Core.RequestHandling.Models;
public class Route
{
    public static Option<Route> From(string? path) =>
        !string.IsNullOrWhiteSpace(path) && path.StartsWith('/')
            ? new Route(path)
            : Option<Route>.None;

    private Route(string path) => Path = new PathString(path);

    public PathString Path { get; }

    public override int GetHashCode() => Path.GetHashCode();
}

public readonly record struct PathString(string Value)
{
    public PathString() : this(string.Empty) { } // Throws

    public string Value { get; } =
        !string.IsNullOrWhiteSpace(Value) && Value.StartsWith('/')
            ? Value
            : throw new ArgumentException("Value cannot be null or whitespace, and must start with '/'.", nameof(Value));

    public static implicit operator string(PathString rootString) => rootString.Value;
}
