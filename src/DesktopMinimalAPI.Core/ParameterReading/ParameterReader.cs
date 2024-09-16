using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Text.Json;

namespace DesktopMinimalAPI.Core.ParameterReading;
internal static class ParameterReader
{
    internal static Option<T> GetParameter<T, U>(ImmutableArray<UrlParameterString> urlParameters, JsonBody body, int parameterIndex)
        where T: ParameterSource<U> => 
        typeof(T) switch
        {
            var t when t == typeof(FromUrl<U>) => Option<T>.Some(new FromUrl<U>(GetUrlParameter<U>(urlParameters, parameterIndex).ValueUnsafe()) as T),
            var t when t == typeof(FromBody<U>) => Option<T>.Some(new FromBody<U>(GetBodyParameter<U>(body).ValueUnsafe()) as T),
            _ => Option<T>.None
        };

    private static Option<T> GetUrlParameter<T>(ImmutableArray<UrlParameterString> urlParameters, int parameterPosition) =>
        (T)Convert.ChangeType(urlParameters[parameterPosition].ToString(), typeof(T), CultureInfo.InvariantCulture);

    private static Option<T> GetBodyParameter<T>(JsonBody body) =>
        JsonSerializer.Deserialize<T>(body.Value, Serialization.DefaultCamelCase) is T deserialized
        ? Option<T>.Some(deserialized)
        : Option<T>.None;
}
