namespace DesktopMinimalAPI.Core.Models;
public sealed class FromUrl<T>
{
    internal FromUrl(T value) => Value = value;

    public T Value { get; }

    public static implicit operator T(FromUrl<T> wrapper) => wrapper.Value;
}
