using SocketPulse.Receiver.CommandGeneration;
using SocketPulse.Receiver.Interfaces;
using SocketPulse.Shared;

namespace SocketPulse.Receiver.Test.Integration.Models;

public class TestAction : IAction
{
    public State Execute(List<string> arguments)
    {
        return State.Success;
    }
}