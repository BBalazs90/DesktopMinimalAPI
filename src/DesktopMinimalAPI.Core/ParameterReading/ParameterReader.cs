using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using LanguageExt;
using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using static LanguageExt.Prelude;

namespace DesktopMinimalAPI.Core.ParameterReading;
internal static class ParameterReader
{
    public static Option<T> GetUrlParameter<T>(ImmutableArray<UrlParameterString> urlParameters) =>
        urlParameters.Length > 0 ?
        TryConvertTo<T>(urlParameters[0].ToString())
            .Match(Some: Option<T>.Some,
                   None: () => TryReadFromString<T>(urlParameters[0].ToString()))
        : Option<T>.None;

    public static Option<(T1, T2)> GetUrlParameters<T1, T2>(ImmutableArray<UrlParameterString> urlParameters) =>
        ((T1)Convert.ChangeType(urlParameters[0].ToString(), typeof(T1), CultureInfo.InvariantCulture),
        (T2)Convert.ChangeType(urlParameters[1].ToString(), typeof(T2), CultureInfo.InvariantCulture));

    public static Option<(T1, T2, T3)> GetUrlParameters<T1, T2, T3>(ImmutableArray<UrlParameterString> urlParameters) =>
        ((T1)Convert.ChangeType(urlParameters[0].ToString(), typeof(T1), CultureInfo.InvariantCulture),
        (T2)Convert.ChangeType(urlParameters[1].ToString(), typeof(T2), CultureInfo.InvariantCulture),
        (T3)Convert.ChangeType(urlParameters[2].ToString(), typeof(T3), CultureInfo.InvariantCulture));

    public static Option<(T1, T2, T3, T4)> GetUrlParameters<T1, T2, T3, T4>(ImmutableArray<UrlParameterString> urlParameters) =>
        ((T1)Convert.ChangeType(urlParameters[0].ToString(), typeof(T1), CultureInfo.InvariantCulture),
        (T2)Convert.ChangeType(urlParameters[1].ToString(), typeof(T2), CultureInfo.InvariantCulture),
        (T3)Convert.ChangeType(urlParameters[2].ToString(), typeof(T3), CultureInfo.InvariantCulture),
        (T4)Convert.ChangeType(urlParameters[3].ToString(), typeof(T4), CultureInfo.InvariantCulture));

    public static Option<T> GetBodyParameter<T>(JsonBody body) =>
        JsonSerializer.Deserialize<T>(body.Value, Serialization.DefaultCamelCase) is T deserialized
        ? Option<T>.Some(deserialized)
        : Option<T>.None;

    private static Option<T> TryConvertTo<T>(string value) =>
        TypeDescriptor.GetConverter(typeof(T)).CanConvertFrom(typeof(string))
        ? (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(value)!
        : Option<T>.None;

    private static Option<T> TryReadFromString<T>(string value) =>
        Try(() => JsonSerializer.Deserialize<T>(value) ?? throw new JsonException($"Could not deserialize from {value}")).ToOption();

}
