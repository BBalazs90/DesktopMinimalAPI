using DesktopMinimalAPI.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DesktopMinimalAPI.Extensions;

public static class WebMessageBrokerCoreExtensions
{
    public static WebMessageBrokerCore MapGet(this WebMessageBrokerCore webMessageBroker, string route, Action handler) => webMessageBroker.MapGetInternal((StringRoute)route, new ActionRegisteredFunction(handler));
    public static WebMessageBrokerCore MapGet(this WebMessageBrokerCore webMessageBroker, string route, Action<string> handler) => webMessageBroker.MapGetInternal((StringRoute)route, new ActionParamRegisteredFunction(handler));
    public static WebMessageBrokerCore MapGet(this WebMessageBrokerCore webMessageBroker, string route, Func<string> handler) => webMessageBroker.MapGetInternal((StringRoute)route, new FuncRegisteredFunction(handler));
    public static WebMessageBrokerCore MapGet(this WebMessageBrokerCore webMessageBroker, string route, Func<string, string> handler) => webMessageBroker.MapGetInternal((StringRoute)route, new FuncParamRegisteredFunction(handler));
    private static WebMessageBrokerCore MapGetInternal(this WebMessageBrokerCore webMessageBroker, IRoute route, IRegisteredFunction handler)
    {
        webMessageBroker.MessageHandlers.Add(route, handler);
        return webMessageBroker;
    }
}