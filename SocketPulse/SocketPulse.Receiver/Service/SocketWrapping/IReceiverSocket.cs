namespace SocketPulse.Receiver.Service.SocketWrapping;

public interface IReceiverSocket
{
    public void InitSocket(string address);
    public string ReceiveFrameString();
    public void SendFrame(string frame);
    public void Close();
}