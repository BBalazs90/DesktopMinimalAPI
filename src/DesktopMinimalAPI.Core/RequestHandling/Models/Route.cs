using LanguageExt;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace DesktopMinimalAPI.Core.RequestHandling.Models;
public sealed class Route : IEquatable<Route>
{
    public static Option<Route> From(string? maybeRoute) => maybeRoute is not null && maybeRoute.StartsWith('/')
        ? new Route(GetPath(maybeRoute), GetParameters(maybeRoute))
        : Option<Route>.None;

    static PathString GetPath(string route) => route.Contains('?', StringComparison.Ordinal)
        ? new(string.Concat(route.TakeWhile(c => c != '?')))
        : new(route);
    static ImmutableArray<UrlParameterString> GetParameters(string route) => route.Contains('?', StringComparison.Ordinal)
        ? route.Split('?')[1]
        .Split('&')
        .Aggregate(ImmutableArray<UrlParameterString>.Empty, (accu, paramPart) => accu.Add(new(paramPart.Split('=')[1])))
        : [];

    private Route(PathString path, ImmutableArray<UrlParameterString> parameters) =>
        (Path, Parameters) = (path, parameters);

    public PathString Path { get; }

    public ImmutableArray<UrlParameterString> Parameters { get; }

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
