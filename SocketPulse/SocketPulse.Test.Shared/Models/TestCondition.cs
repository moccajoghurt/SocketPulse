using SocketPulse.Receiver.Interfaces;

namespace SocketPulse.Test.Shared.Models;

public class TestCondition : ICondition
{
    public bool Execute(Dictionary<string, string> arguments)
    {
        return true;
    }
}