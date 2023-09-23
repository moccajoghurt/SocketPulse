using SocketPulse.Receiver.Interfaces;
using SocketPulse.Receiver.Service;

namespace SocketPulse.Receiver.Nodes;

public class GetStateInfo : IData
{
    public string Execute(Dictionary<string, string> arguments)
    {
        return SocketPulseReceiverSettings.GetStateInfo.Aggregate("", (current, s) => current + s.Invoke());
    }
}