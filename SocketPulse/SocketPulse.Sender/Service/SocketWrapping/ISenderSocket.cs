using SocketPulse.Shared;

namespace SocketPulse.Sender.Service.SocketWrapping;

public interface ISenderSocket
{
    public bool Connect(string address);
    public Reply SendRequest(Request request);
    public bool TrySendRequest(Request request, TimeSpan timeout, out Reply? reply);
    public void Close();
    public uint GetTickRate();
    public NodeInfo GetAllNodes();
}