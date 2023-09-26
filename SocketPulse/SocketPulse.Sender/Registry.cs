using Microsoft.Extensions.DependencyInjection;
using SocketPulse.Sender.Service;
using SocketPulse.Sender.Service.SocketWrapping;

namespace SocketPulse.Sender;

public static class Registry
{
    public static void AddSocketPulseSender(this IServiceCollection services)
    {
        services.AddSingleton<ISenderSocket, SenderSocket>();
    }
}