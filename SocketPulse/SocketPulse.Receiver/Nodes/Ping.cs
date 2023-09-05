using SocketPulse.Receiver.Interfaces;

namespace SocketPulse.Receiver.Nodes;

public class Ping : IData
{
    public string Execute(Dictionary<string, string> arguments)
    {
        return "pong";
    }
}