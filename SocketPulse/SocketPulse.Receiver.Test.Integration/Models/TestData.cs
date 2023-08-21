using SocketPulse.Receiver.Interfaces;

namespace SocketPulse.Receiver.Test.Integration.Models;

public class TestData : IData
{
    public string Execute(List<string> arguments)
    {
        return "foo";
    }
}