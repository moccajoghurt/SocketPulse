using SocketPulse.Shared;

namespace SocketPulse.Sender.Service.SocketWrapping;

public interface ISenderSocket
{
    public bool Connect(string address);
    public Reply SendRequest(Request request, TimeSpan? timeout = null);
    public void Close();
    public uint GetTickRate();
    public NodeInfo GetAllNodes();
}