using SocketPulse.Receiver.Interfaces;
using SocketPulse.Receiver.Service;

namespace SocketPulse.Receiver.Nodes;

//
// The sender endpoint is likely executing a behavior tree to dispatch commands to the receiver endpoint.
// As the receiver, you can specify your preferred tick rate for this behavior tree.
// The sender is expected to retrieve and adhere to this specified tick rate.
//
public class GetTickRate : IData
{
    public string Execute(List<string> arguments)
    {
        return SocketPulseReceiverSettings.TickRateMs.ToString();
    }
}