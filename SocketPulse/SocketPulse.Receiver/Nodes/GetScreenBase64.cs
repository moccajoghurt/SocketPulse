using SocketPulse.Receiver.Interfaces;
using SocketPulse.Receiver.Service;

namespace SocketPulse.Receiver.Nodes;

public class GetScreenBase64 : IData
{
    public string Execute(Dictionary<string, string> arguments)
    {
        return SocketPulseReceiverSettings.GetScreenBase64();
    }
}