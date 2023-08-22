using SocketPulse.Receiver.Interfaces;

namespace SocketPulse.Test.Shared.Models;

public class TestCondition : ICondition
{
    public bool Execute(List<string> arguments)
    {
        return true;
    }
}