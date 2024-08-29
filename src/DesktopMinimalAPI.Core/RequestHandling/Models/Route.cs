using DesktopMinimalAPI.Core.RequestHandling.Models.Exceptions;
using LanguageExt;
using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DesktopMinimalAPI.Core.RequestHandling.Models;


public readonly struct Route : IEquatable<Route>
{
    public static Either<ArgumentException, Route> From(string? maybeRoute) =>
        from path in GetPath(maybeRoute)
        from parameters in GetParameters(maybeRoute![path.Value.Length..]) // By this time GetPath would have failed if maybeRoute is null, ok to use !
        select new Route(path, parameters);

    static Either<ArgumentException, PathString> GetPath(string? maybeRoute) => maybeRoute switch
    {
        string maybeRouteNotNull when maybeRouteNotNull.StartsWith('/') && maybeRouteNotNull.Contains('?', StringComparison.Ordinal) =>
            new PathString(string.Concat(maybeRouteNotNull.TakeWhile(c => c != '?'))),

        string maybeRouteNotNull when maybeRouteNotNull.StartsWith('/') => new PathString(maybeRouteNotNull),

        _ => new ArgumentException($"Valid route must be not null, not whitespace and start with '/'. Got '{maybeRoute}'", nameof(maybeRoute))
    };

    static Either<ArgumentException, ImmutableArray<UrlParameterString>> GetParameters(string maybeRoute) => maybeRoute switch
    {
        _ when maybeRoute.Length > 0 => maybeRoute
            .Split('&')
            .Select(strPart => new UrlParameterString(string.Join(string.Empty, strPart.SkipWhile(c => c != '=').Skip(1))))
            .ToImmutableArray(),
        _ => []
    };
       

    private Route(PathString path, ImmutableArray<UrlParameterString> parameters) =>
        (Path, Parameters) = (path, parameters);

    public PathString Path { get; }

    public ImmutableArray<UrlParameterString> Parameters { get; }

    public override int GetHashCode() => Path.GetHashCode();

    public override bool Equals(object? obj) => obj is Route route && Equals(route);

    public bool Equals(Route other) => Path.Equals(other.Path);
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
