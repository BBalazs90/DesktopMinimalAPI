using System;
using System.Collections.Immutable;

namespace DesktopMinimalAPI.Core.Models;

internal sealed class TransformedWmRequest(Guid Id, ImmutableArray<RequestParameterInfo> ParameterInfos)
{
    public Guid Id { get; } = Id;
    public ImmutableArray<RequestParameterInfo> ParameterInfos { get; } = ParameterInfos;
}
