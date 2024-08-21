using DesktopMinimalAPI.Core.RequestHandling.Models.Methods;
using System;
using System.Collections.Immutable;

namespace DesktopMinimalAPI.Core.Models;

internal sealed class TransformedWmRequest(Guid Id, Method Method, ImmutableArray<RequestParameterIntermediate> ParameterInfos)
{
    public Guid Id { get; } = Id;
    public Method Method { get; } = Method;
    public ImmutableArray<RequestParameterIntermediate> ParameterInfos { get; } = ParameterInfos;
}
