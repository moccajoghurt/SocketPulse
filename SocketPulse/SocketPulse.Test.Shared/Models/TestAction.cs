using SocketPulse.Receiver.Interfaces;
using SocketPulse.Shared;

namespace SocketPulse.Test.Shared.Models;

public class TestAction : IAction
{
    public State Execute(List<string> arguments)
    {
        return State.Success;
    }
}