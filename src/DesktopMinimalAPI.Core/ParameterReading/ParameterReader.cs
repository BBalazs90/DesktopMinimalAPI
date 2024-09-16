using DesktopMinimalAPI.Core.RequestHandling.Models;
using LanguageExt;
using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Reflection.Metadata;

namespace DesktopMinimalAPI.Core.ParameterReading;
internal static class ParameterReader
{
    internal static Option<T> GetParameter<T>(ImmutableArray<UrlParameterString> urlParameters, JsonBody body) =>
        (T)Convert.ChangeType(urlParameters[0].ToString(), typeof(T), CultureInfo.InvariantCulture);
}
