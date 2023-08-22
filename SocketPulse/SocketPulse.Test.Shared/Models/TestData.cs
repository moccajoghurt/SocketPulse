using SocketPulse.Receiver.Interfaces;

namespace SocketPulse.Test.Shared.Models;

public class TestData : IData
{
    public string Execute(List<string> arguments)
    {
        return "foo";
    }
}