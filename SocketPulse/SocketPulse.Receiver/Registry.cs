using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SocketPulse.Receiver.CommandGeneration;
using SocketPulse.Receiver.CommandInvocation;
using SocketPulse.Receiver.Interfaces;
using SocketPulse.Receiver.Service;
using SocketPulse.Receiver.Service.SocketWrapping;

namespace SocketPulse.Receiver;

public static class Registry
{
    public static void AddSocketPulseReceiver(this IServiceCollection services, List<Assembly> assemblies)
    {
        services.AddSingleton<ICommandGenerator, CommandGenerator>();
        services.AddSingleton<ICommandInvoker, CommandInvoker>();
        services.AddSingleton<IReceiverSocket, ReceiverSocket>();
        services.AddSingleton<ISocketPulseReceiver, SocketPulseReceiver>();

        RegisterCommandType(services, typeof(ICondition), assemblies);
        RegisterCommandType(services, typeof(IAction), assemblies);
        RegisterCommandType(services, typeof(IData), assemblies);
    }

    private static void RegisterCommandType(IServiceCollection services, Type type, ICollection<Assembly> assemblies)
    {
        assemblies.Add(typeof(Registry).Assembly);
        var types = assemblies.SelectMany(s => s.GetTypes())
            .Where(t => type.IsAssignableFrom(t) && t.IsClass).ToList();
        foreach (var t in types)
        {
            services.AddSingleton(t);
        }
    }
}