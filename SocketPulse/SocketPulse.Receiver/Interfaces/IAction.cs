using SocketPulse.Shared;

namespace SocketPulse.Receiver.Interfaces;
public interface IAction
{
    public State Execute(List<string> arguments);
}
