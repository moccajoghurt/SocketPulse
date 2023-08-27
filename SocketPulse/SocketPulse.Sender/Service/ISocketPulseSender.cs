using SocketPulse.Shared;

namespace SocketPulse.Sender.Service;

public interface ISocketPulseSender
{
    public bool Connect(string address, bool printErrors = true);
    public void Disconnect();
    public Reply SendRequest(Request request);
    public uint GetTickRate();
    public NodeInfo GetAllNodes();
}