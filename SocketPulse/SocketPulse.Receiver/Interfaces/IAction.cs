using SocketPulse.Shared;

namespace SocketPulse.Receiver.Interfaces;
public interface IAction
{
    public State Execute(Dictionary<string, string> arguments);
}
