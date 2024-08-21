using LanguageExt;
using LanguageExt.ClassInstances.Pred;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DesktopMinimalAPI.Core.RequestHandling.Models;
public sealed class Route : IEquatable<Route>
{
    public static Option<Route> From(string? path) =>
        !string.IsNullOrWhiteSpace(path) && path.StartsWith('/')
            ? new Route(path)
            : Option<Route>.None;

    private Route(string path) => Path = new PathString(path);

    public PathString Path { get; }

    public override int GetHashCode() => Path.GetHashCode();

    public override bool Equals(object? obj) => obj is Route route && Equals(route);

    public bool Equals(Route? other) => Path.Equals(other?.Path);
}

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
