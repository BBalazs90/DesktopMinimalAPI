using DesktopMinimalAPI.Core.Abstractions;
using DesktopMinimalAPI.Core.Models;
using DesktopMinimalAPI.Models;
using System;
using System.Threading.Tasks;
using static DesktopMinimalAPI.Core.HandlerPipeline;
using static DesktopMinimalAPI.Core.RoutePipeline;

namespace DesktopMinimalAPI.Extensions;

public static class HandlerBuilderExtensions
{
    private static Func<T, HandlerBuilderBase> ApplyRoute<T>(string route, Func<IRoute, T, HandlerBuilderBase> f) =>
        (T handler) => f(GetRoot(route), handler);

    public static HandlerBuilderBase MapGet<Tout>(this HandlerBuilderBase builder, string route, Func<Tout> handler) =>
        ApplyRoute<Func<TransformedWmRequest, WmResponse>>(route, builder.MapGet)(Transform(handler));
    
    public static HandlerBuilderBase MapGet<Tin, Tout>(this HandlerBuilderBase builder, string route, Func<Tin, Tout> handler) =>
      ApplyRoute<Func<TransformedWmRequest, WmResponse>>(route, builder.MapGet)(Transform(handler));

    public static HandlerBuilderBase MapGet<Tin1, Tin2, Tout>(this HandlerBuilderBase builder, string route, Func<Tin1, Tin2, Tout> handler) =>
       ApplyRoute<Func<TransformedWmRequest, WmResponse>>(route, builder.MapGet)(Transform(handler));

    //public static HandlerBuilderBase MapGet<T>(this HandlerBuilderBase builder, string route, Func<Task<T>> handler) =>
    //    ApplyRoute<Func<TransformedWmRequest, WmResponse>>(route, builder.MapGet)(Transform(handler));


}