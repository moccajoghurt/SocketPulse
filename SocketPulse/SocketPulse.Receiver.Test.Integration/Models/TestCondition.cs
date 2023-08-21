using SocketPulse.Receiver.Interfaces;

namespace SocketPulse.Receiver.Test.Integration.Models;

public class TestCondition : ICondition
{
    public bool Execute(List<string> arguments)
    {
        return true;
    }
}