using DesktopMinimalAPI.Core.Configuration;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using LanguageExt;
using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Text.Json;

namespace DesktopMinimalAPI.Core.ParameterReading;
internal static class ParameterReader
{
    public static Option<T> GetUrlParameter<T>(ImmutableArray<UrlParameterString> urlParameters) =>
        (T)Convert.ChangeType(urlParameters[0].ToString(), typeof(T), CultureInfo.InvariantCulture);

    public static Option<(T1, T2)> GetUrlParameters<T1, T2>(ImmutableArray<UrlParameterString> urlParameters) =>
        ((T1)Convert.ChangeType(urlParameters[0].ToString(), typeof(T1), CultureInfo.InvariantCulture),
        (T2)Convert.ChangeType(urlParameters[0].ToString(), typeof(T2), CultureInfo.InvariantCulture));

    public static Option<T> GetBodyParameter<T>(JsonBody body) =>
        JsonSerializer.Deserialize<T>(body.Value, Serialization.DefaultCamelCase) is T deserialized
        ? Option<T>.Some(deserialized)
        : Option<T>.None;
}
