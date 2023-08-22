using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using SocketPulse.Shared;

namespace SocketPulse.Sender.Service.SocketWrapping;

public class SenderSocket : ISenderSocket
{
    private RequestSocket? _requestSocket;
    public bool InitSocket(string address)
    {
        if (_requestSocket == null)
        {
            _requestSocket = new RequestSocket(address);
        }
        else
        {
            _requestSocket.Close();
            _requestSocket = new RequestSocket(address);
        }

        var ping = new Request
        {
            Name = "Ping",
            Type = RequestType.Data
        };
        _requestSocket.SendFrame(JsonConvert.SerializeObject(ping));
        var success = _requestSocket.TryReceiveFrameString(TimeSpan.FromSeconds(10), out var result);
        if (!success || result == null) return false;
        var reply = JsonConvert.DeserializeObject<Reply>(result);
        return reply?.Content == "pong";
    }

    public string ReceiveFrameString()
    {
        return (_requestSocket ?? throw new InvalidOperationException("Socket not initialized")).ReceiveFrameString();
    }

    public void SendFrame(string frame)
    {
        (_requestSocket ?? throw new InvalidOperationException("Socket not initialized")).SendFrame(frame);
    }

    public void Close()
    {
        _requestSocket?.Close();
    }
}