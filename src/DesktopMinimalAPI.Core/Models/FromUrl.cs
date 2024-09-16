namespace DesktopMinimalAPI.Core.Models;

public abstract record ParameterSource<T>;

public sealed record FromUrl<T>(T Value) : ParameterSource<T>
{
    public static implicit operator T(FromUrl<T> wrapper) => wrapper.Value;
}

public sealed record FromBody<T>(T Value) : ParameterSource<T>
{
    public static implicit operator T(FromBody<T> wrapper) => wrapper.Value;
}
