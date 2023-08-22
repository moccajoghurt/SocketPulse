namespace SocketPulse.Sender.Service.SocketWrapping;

public interface ISenderSocket
{
    public bool InitSocket(string address);
    public string ReceiveFrameString();
    public void SendFrame(string frame);
    public void Close();
}