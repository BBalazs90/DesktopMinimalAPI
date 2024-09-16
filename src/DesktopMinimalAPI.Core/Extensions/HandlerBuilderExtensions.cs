using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using DesktopMinimalAPI.Models;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using DesktopMinimalAPI.Core.HandlerRegistration;
using static DesktopMinimalAPI.Core.HandlerPipeline;
using static DesktopMinimalAPI.Core.HandlerRegistration.Sync.SyncHandlerTransformer;
using static DesktopMinimalAPI.Core.HandlerRegistration.EitherBased.Async.EitherAsyncHandlerTransformer;
using static DesktopMinimalAPI.Core.HandlerRegistration.EitherBased.Sync.EitherSyncHandlerTransformer;

namespace DesktopMinimalAPI.Extensions;

[SuppressMessage("Usage", "CA1062:Validate arguments of public methods",
    Justification = "These are extensions methods, it is guranteed that the extended object will not be null.")]
public static class HandlerBuilderExtensions
{
    private static Func<T, HandlerBuilderBase> ApplyRoute<T>(string route, Func<Route, T, HandlerBuilderBase> f) =>
        (T handler) => f(Route.From(route).ValueUnsafe(), handler); // TODO: Fix ValueUnsafe

    public static HandlerBuilderBase MapGet<TOut>(this HandlerBuilderBase builder, string route, Func<TOut> handler) =>
        ApplyRoute<Func<WmRequest, WmResponse>>(route, builder.MapGet)(Transform(handler));

    public static HandlerBuilderBase MapGet<TEx, TOut>(this HandlerBuilderBase builder, string route, Func<Either<TEx, TOut>> handler)
        where TEx: Exception=>
        ApplyRoute<Func<WmRequest, WmResponse>>(route, builder.MapGet)(Transform(handler));

    public static HandlerBuilderBase MapGet<TOut>(this HandlerBuilderBase builder, string route, Func<Task<TOut>> handler) =>
        ApplyRoute<Func<WmRequest, Task<WmResponse>>>(route, builder.MapGet)(Transform(handler));

    public static HandlerBuilderBase MapGet<TEx, TOut>(this HandlerBuilderBase builder, string route, Func<Task<Either<TEx, TOut>>> handler)
        where TEx : Exception =>
        ApplyRoute<Func<WmRequest, Task<WmResponse>>>(route, builder.MapGet)(Transform(handler));

    public static HandlerBuilderBase MapGet<TIn, TEx, TOut>(this HandlerBuilderBase builder, string route, Func<TIn, Task<Either<TEx, TOut>>> handler)
        where TEx : Exception =>
        ApplyRoute<Func<WmRequest, Task<WmResponse>>>(route, builder.MapGet)(Transform(handler));

    public static HandlerBuilderBase MapGet<TIn, TOut>(this HandlerBuilderBase builder, string route, Func<TIn, TOut> handler) =>
      ApplyRoute<Func<WmRequest, WmResponse>>(route, builder.MapGet)(Transform(handler));

    public static HandlerBuilderBase MapGet<TIn1, TIn2, TOut>(this HandlerBuilderBase builder, string route, Func<TIn1, TIn2, TOut> handler) =>
       ApplyRoute<Func<WmRequest, WmResponse>>(route, builder.MapGet)(Transform(handler));

    public static HandlerBuilderBase MapGet<TIn1, TIn2, TIn3, TOut>(this HandlerBuilderBase builder, string route, Func<TIn1, TIn2, TIn3, TOut> handler) =>
       ApplyRoute<Func<WmRequest, WmResponse>>(route, builder.MapGet)(Transform(handler));

    public static HandlerBuilderBase MapPost<TOut>(this HandlerBuilderBase builder, string route, Func<TOut> handler) =>
        ApplyRoute<Func<WmRequest, WmResponse>>(route, builder.MapPost)(Transform(handler));

    public static HandlerBuilderBase MapPost<TIn, TOut>(this HandlerBuilderBase builder, string route, Func<TIn, TOut> handler) =>
      ApplyRoute<Func<WmRequest, WmResponse>>(route, builder.MapPost)(Transform(handler));

    public static HandlerBuilderBase MapPost<TIn1, TIn2, TOut>(this HandlerBuilderBase builder, string route, Func<TIn1, TIn2, TOut> handler) =>
       ApplyRoute<Func<WmRequest, WmResponse>>(route, builder.MapPost)(Transform(handler));

    public static HandlerBuilderBase MapPost<TIn1, TIn2, TIn3, TOut>(this HandlerBuilderBase builder, string route, Func<TIn1, TIn2, TIn3, TOut> handler) =>
       ApplyRoute<Func<WmRequest, WmResponse>>(route, builder.MapPost)(Transform(handler));
}
