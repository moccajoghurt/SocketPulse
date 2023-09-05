using SocketPulse.Receiver.Interfaces;

namespace SocketPulse.Test.Shared.Models;

public class TestData : IData
{
    public string Execute(Dictionary<string, string> arguments)
    {
        return "foo";
    }
}