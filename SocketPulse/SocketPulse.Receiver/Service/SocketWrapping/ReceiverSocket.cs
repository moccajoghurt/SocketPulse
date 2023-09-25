using NetMQ;
using NetMQ.Sockets;

namespace SocketPulse.Receiver.Service.SocketWrapping;

public class ReceiverSocket : IReceiverSocket
{
    private RouterSocket? _routerSocket;

    public void InitSocket(string address)
    {
        if (_routerSocket == null)
        {
            _routerSocket = new RouterSocket(address);
        }
        else
        {
            _routerSocket.Close();
            _routerSocket = new RouterSocket(address);
        }
    }

    public (string senderIdentity, string message) ReceiveFrameString()
    {
        var senderIdentity = (_routerSocket ?? throw new InvalidOperationException("Socket not initialized"))
            .ReceiveFrameString();
        var message = _routerSocket.ReceiveFrameString();
        return (senderIdentity, message);
    }

    public void SendFrame(string senderIdentity, string frame)
    {
        var socket = _routerSocket ?? throw new InvalidOperationException("Socket not initialized");
        socket.SendMoreFrame(senderIdentity).SendFrame(frame);
    }

    public void Close()
    {
        _routerSocket?.Close();
    }
}