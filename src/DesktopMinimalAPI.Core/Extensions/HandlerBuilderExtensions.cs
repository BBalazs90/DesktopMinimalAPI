using DesktopMinimalAPI.Core.Abstractions;
using DesktopMinimalAPI.Models;
using System;
using System.Threading.Tasks;
using static DesktopMinimalAPI.HandlerPipeline;

namespace DesktopMinimalAPI.Extensions;

public static class HandlerBuilderExtensions
{
    public static HandlerBuilderBase MapGet(this HandlerBuilderBase builder, string route, Func<WmRequest, WmResponse> handler) => 
        builder.MapGet((StringRoute)route, handler);

    public static HandlerBuilderBase MapGet<T>(this HandlerBuilderBase builder, string route, Func<T> handler) =>
        builder.MapGet((StringRoute) route, Transform(handler));

    public static HandlerBuilderBase MapGet<T>(this HandlerBuilderBase builder, string route, Func<Task<T>> handler) =>
        builder.MapGet((StringRoute)route, Transform(handler));


}