using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using SocketPulse.Shared;

namespace SocketPulse.Sender.Service.SocketWrapping;

public class SenderSocket : ISenderSocket
{
    private DealerSocket? _dealerSocket;

    public bool InitSocket(string address)
    {
        if (_dealerSocket == null)
        {
            _dealerSocket = new DealerSocket(address);
        }
        else
        {
            _dealerSocket.Close();
            _dealerSocket = new DealerSocket(address);
        }

        var ping = new Request
        {
            Name = "Ping",
            Type = RequestType.Data
        };

        _dealerSocket.SendFrame(JsonConvert.SerializeObject(ping));

        var success = _dealerSocket.TryReceiveFrameString(TimeSpan.FromSeconds(3), out var result);
        if (!success) return false;

        var reply = JsonConvert.DeserializeObject<Reply>(result!);
        return reply?.Content == "pong";
    }

    public string ReceiveFrameString()
    {
        return (_dealerSocket ?? throw new InvalidOperationException("Socket not initialized")).ReceiveFrameString();
    }

    public void SendFrame(string frame)
    {
        (_dealerSocket ?? throw new InvalidOperationException("Socket not initialized")).SendFrame(frame);
    }

    public void Close()
    {
        _dealerSocket?.Close();
    }
}
