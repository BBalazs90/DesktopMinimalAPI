using System;
using System.Diagnostics.CodeAnalysis;
using DesktopMinimalAPI.Core.Abstractions;
using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Models;
using static DesktopMinimalAPI.Core.HandlerPipeline;
using static DesktopMinimalAPI.Core.RoutePipeline;

namespace DesktopMinimalAPI.Extensions;

[SuppressMessage("Usage", "CA1062:Validate arguments of public methods",
    Justification = "These are extensions methods, it is guranteed that the extended object will not be null.")]
public static class HandlerBuilderExtensions
{
    private static Func<T, HandlerBuilderBase> ApplyRoute<T>(string route, Func<IRoute, T, HandlerBuilderBase> f) =>
        (T handler) => f(GetRoot(route), handler);

    public static HandlerBuilderBase MapGet<TOut>(this HandlerBuilderBase builder, string route, Func<TOut> handler) =>
        ApplyRoute<Func<TransformedWmRequest, WmResponse>>(route, builder.MapGet)(Transform(handler));
    
    public static HandlerBuilderBase MapGet<TIn, TOut>(this HandlerBuilderBase builder, string route, Func<TIn, TOut> handler) =>
      ApplyRoute<Func<TransformedWmRequest, WmResponse>>(route, builder.MapGet)(Transform(handler));

    public static HandlerBuilderBase MapGet<TIn1, TIn2, TOut>(this HandlerBuilderBase builder, string route, Func<TIn1, TIn2, TOut> handler) =>
       ApplyRoute<Func<TransformedWmRequest, WmResponse>>(route, builder.MapGet)(Transform(handler));

    public static HandlerBuilderBase MapGet<TIn1, TIn2, TIn3, TOut>(this HandlerBuilderBase builder, string route, Func<TIn1, TIn2, TIn3, TOut> handler) =>
       ApplyRoute<Func<TransformedWmRequest, WmResponse>>(route, builder.MapGet)(Transform(handler));

    public static HandlerBuilderBase MapPost<TOut>(this HandlerBuilderBase builder, string route, Func<TOut> handler) =>
        ApplyRoute<Func<TransformedWmRequest, WmResponse>>(route, builder.MapPost)(Transform(handler));

    public static HandlerBuilderBase MapPost<TIn, TOut>(this HandlerBuilderBase builder, string route, Func<TIn, TOut> handler) =>
      ApplyRoute<Func<TransformedWmRequest, WmResponse>>(route, builder.MapPost)(Transform(handler));

    public static HandlerBuilderBase MapPost<TIn1, TIn2, TOut>(this HandlerBuilderBase builder, string route, Func<TIn1, TIn2, TOut> handler) =>
       ApplyRoute<Func<TransformedWmRequest, WmResponse>>(route, builder.MapPost)(Transform(handler));

    public static HandlerBuilderBase MapPost<TIn1, TIn2, TIn3, TOut>(this HandlerBuilderBase builder, string route, Func<TIn1, TIn2, TIn3, TOut> handler) =>
       ApplyRoute<Func<TransformedWmRequest, WmResponse>>(route, builder.MapPost)(Transform(handler));
}
