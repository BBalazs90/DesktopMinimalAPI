using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Core.RequestHandling.Models;
using DesktopMinimalAPI.Models;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using static DesktopMinimalAPI.Core.HandlerRegistration.EitherBased.Async.EitherAsyncHandlerTransformer;
using static System.Net.Mime.MediaTypeNames;

namespace DesktopMinimalAPI.Core.HandlerRegistration.EitherBased.Async;

[SuppressMessage("Usage", "CA1062:Validate arguments of public methods",
    Justification = "These are extensions methods, it is guranteed that the extended object will not be null.")]
public static class BuilderExtensions
{
    private static Func<T, HandlerBuilderBase> ApplyRoute<T>(string route, Func<Route, T, HandlerBuilderBase> f) =>
        (T handler) => f(Route.From(route).ValueUnsafe(), handler); // TODO: Fix ValueUnsafe

    public static HandlerBuilderBase MapGet<TEx, TOut>(this HandlerBuilderBase builder, string route, Func<Task<Either<TEx, TOut>>> handler)
       where TEx : Exception =>
       ApplyRoute<Func<WmRequest, Task<WmResponse>>>(route, builder.MapGet)(Transform(handler));

    public static HandlerBuilderBase MapGet<TP, TIn, TEx, TOut>(this HandlerBuilderBase builder, string route, Func<TP, Task<Either<TEx, TOut>>> handler)
        where TP : ParameterSource<TIn>
        where TEx : Exception =>
        ApplyRoute<Func<WmRequest, Task<WmResponse>>>(route, builder.MapGet)(Transform<TP, TIn, TEx, TOut>(handler));

    public static HandlerBuilderBase MapGet<TP1, TP2, TIn1, TIn2, TEx, TOut>(this HandlerBuilderBase builder, string route, Func<TP1, TP2, Task<Either<TEx, TOut>>> handler)
        where TP1 : ParameterSource<TIn1>
        where TP2 : ParameterSource<TIn2>
        where TEx : Exception =>
     ApplyRoute<Func<WmRequest, Task<WmResponse>>>(route, builder.MapGet)(Transform<TP1, TP2, TIn1, TIn2, TEx, TOut>(handler));


    public static HandlerBuilderBase MapPost<TEx, TOut>(this HandlerBuilderBase builder, string route, Func<Task<Either<TEx, TOut>>> handler)
        where TEx : Exception =>
        ApplyRoute<Func<WmRequest, Task<WmResponse>>>(route, builder.MapPost)(Transform(handler));


    public static HandlerBuilderBase MapPost<TP, TIn, TEx, TOut>(this HandlerBuilderBase builder, string route, Func<TP, Task<Either<TEx, TOut>>> handler)
        where TP : ParameterSource<TIn>
        where TEx : Exception =>
      ApplyRoute<Func<WmRequest, Task<WmResponse>>>(route, builder.MapPost)(Transform<TP, TIn, TEx, TOut>(handler));

    public static HandlerBuilderBase MapPost<TP1, TP2, TIn1, TIn2, TEx, TOut>(this HandlerBuilderBase builder, string route, Func<TP1, TP2, Task<Either<TEx, TOut>>> handler)
        where TP1 : ParameterSource<TIn1>
        where TP2 : ParameterSource<TIn2>
        where TEx : Exception =>
       ApplyRoute<Func<WmRequest, Task<WmResponse>>>(route, builder.MapPost)(Transform<TP1, TP2, TIn1, TIn2, TEx, TOut>(handler));
}
