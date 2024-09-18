using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using DesktopMinimalAPI.Models;
using LanguageExt.UnsafeValueAccess;
using System;
using System.Diagnostics.CodeAnalysis;

using static DesktopMinimalAPI.Core.HandlerRegistration.Sync.SyncHandlerTransformer;

namespace DesktopMinimalAPI.Core.HandlerRegistration.Sync;


[SuppressMessage("Usage", "CA1062:Validate arguments of public methods",
    Justification = "These are extensions methods, it is guranteed that the extended object will not be null.")]
public static class BuilderExtensions
{
    private static Func<T, HandlerBuilderBase> ApplyRoute<T>(string route, Func<Route, T, HandlerBuilderBase> f) =>
        (T handler) => f(Route.From(route).ValueUnsafe(), handler); // TODO: Fix ValueUnsafe

    public static HandlerBuilderBase MapGet<TOut>(this HandlerBuilderBase builder, string route, Func<HandlerResult<TOut>> handler) =>
       ApplyRoute<Func<WmRequest, WmResponse>>(route, builder.MapGet)(Transform(handler));

    public static HandlerBuilderBase MapGet<TIn, TOut>(this HandlerBuilderBase builder, string route, Func<FromUrl<TIn>, HandlerResult<TOut>> handler) =>
     ApplyRoute<Func<WmRequest, WmResponse>>(route, builder.MapGet)(Transform(handler));

    public static HandlerBuilderBase MapGet<TIn1, TIn2, TOut>(this HandlerBuilderBase builder, string route, Func<FromUrl<TIn1>, FromUrl<TIn2>, HandlerResult<TOut>> handler) =>
     ApplyRoute<Func<WmRequest, WmResponse>>(route, builder.MapGet)(Transform(handler));


    //public static HandlerBuilderBase MapPost<TOut>(this HandlerBuilderBase builder, string route, Func<TOut> handler) =>
    //    ApplyRoute<Func<WmRequest, WmResponse>>(route, builder.MapPost)(Transform(handler));


    //public static HandlerBuilderBase MapPost<TP, TIn, TOut>(this HandlerBuilderBase builder, string route, Func<TP, TOut> handler)
    //    where TP : ParameterSource<TIn> =>
    //  ApplyRoute<Func<WmRequest, WmResponse>>(route, builder.MapPost)(Transform<TP, TIn, TOut>(handler));

    //public static HandlerBuilderBase MapPost<TP1, TP2, TIn1, TIn2, TOut>(this HandlerBuilderBase builder, string route, Func<TP1, TP2, TOut> handler)
    //    where TP1 : ParameterSource<TIn1>
    //    where TP2 : ParameterSource<TIn2> =>
    //   ApplyRoute<Func<WmRequest, WmResponse>>(route, builder.MapPost)(Transform<TP1, TP2, TIn1, TIn2, TOut>(handler));
}
