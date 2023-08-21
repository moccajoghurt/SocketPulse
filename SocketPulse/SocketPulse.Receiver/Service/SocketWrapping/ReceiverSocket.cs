using NetMQ;
using NetMQ.Sockets;

namespace SocketPulse.Receiver.Service.SocketWrapping;

public class ReceiverSocket : IReceiverSocket
{
    private ResponseSocket? _responseSocket;

    public void InitSocket(string address)
    {
        if (_responseSocket == null)
        {
            _responseSocket = new ResponseSocket(address);
        }
        else
        {
            _responseSocket.Close();
            _responseSocket = new ResponseSocket(address);
        }
    }

    public string ReceiveFrameString()
    {
        return (_responseSocket ?? throw new InvalidOperationException("Socket not initialized")).ReceiveFrameString();
    }

    public void SendFrame(string frame)
    {
        (_responseSocket ?? throw new InvalidOperationException("Socket not initialized")).SendFrame(frame);
    }

    public void Close()
    {
        _responseSocket?.Close();
    }
}