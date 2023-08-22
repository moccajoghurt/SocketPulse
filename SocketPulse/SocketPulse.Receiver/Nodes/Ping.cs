using SocketPulse.Receiver.Interfaces;

namespace SocketPulse.Receiver.Nodes;

public class Ping : IData
{
    public string Execute(List<string> arguments)
    {
        return "pong";
    }
}