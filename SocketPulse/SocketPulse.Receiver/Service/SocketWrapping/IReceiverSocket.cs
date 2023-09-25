namespace SocketPulse.Receiver.Service.SocketWrapping;

public interface IReceiverSocket
{
    void InitSocket(string address);
    (string senderIdentity, string message) ReceiveFrameString();
    void SendFrame(string senderIdentity, string frame);
    void Close();
}