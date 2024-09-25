using LanguageExt;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace DesktopMinimalAPI.Core.RequestHandling.Models;

public readonly record struct Route
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

    public Route() => throw new NotSupportedException($"Do not create this type directly, use the provided smart constructor, named {nameof(From)}");

    private Route(PathString path, ImmutableArray<UrlParameterString> parameters) =>
        (Path, Parameters) = (path, parameters);

    public PathString Path { get; }

    public ImmutableArray<UrlParameterString> Parameters { get; }

    public override int GetHashCode() => Path.GetHashCode();

    public bool Equals(Route other) => Path.Equals(other.Path);
}
