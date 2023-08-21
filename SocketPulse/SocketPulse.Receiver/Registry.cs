using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SocketPulse.Receiver.CommandGeneration;
using SocketPulse.Receiver.CommandInvokation;
using SocketPulse.Receiver.Interfaces;
using SocketPulse.Receiver.Service;
using SocketPulse.Receiver.Service.SocketWrapping;

namespace SocketPulse.Receiver;

public static class Registry
{
    public static void AddSocketPulseReceiver(this IServiceCollection services, Assembly[] assemblies)
    {
        services.AddSingleton<ICommandGenerator, CommandGenerator>();
        services.AddSingleton<ICommandInvoker, CommandInvoker>();
        services.AddSingleton<IReceiverSocket, ReceiverSocket>();
        services.AddSingleton<ISocketService, SocketService>();

        RegisterCommandType(services, typeof(ICondition), assemblies);
        RegisterCommandType(services, typeof(IAction), assemblies);
        RegisterCommandType(services, typeof(IData), assemblies);
    }

    private static void RegisterCommandType(IServiceCollection services, Type type, Assembly[] assemblies)
    {
        Console.WriteLine(assemblies[0].GetName().Name);
        var types = assemblies.SelectMany(s => s.GetTypes())
            .Where(t => type.IsAssignableFrom(t) && t.IsClass).ToList();
        foreach (var t in types)
        {
            services.AddScoped(t);
        }
    }
}