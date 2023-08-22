using Newtonsoft.Json;
using SocketPulse.Sender.Service.SocketWrapping;
using SocketPulse.Shared;

namespace SocketPulse.Sender.Service;

public class SocketPulseSender : ISocketPulseSender
{
    private readonly ISenderSocket _senderSocket;

    public SocketPulseSender(ISenderSocket senderSocket)
    {
        _senderSocket = senderSocket;
    }

    public bool Connect(string address)
    {
        return _senderSocket.InitSocket(address);
    }

    public void Disconnect()
    {
        _senderSocket.Close();
    }

    public Reply SendRequest(Request request)
    {
        var requestString = JsonConvert.SerializeObject(request);
        _senderSocket.SendFrame(requestString);
        var replyString = _senderSocket.ReceiveFrameString();
        return JsonConvert.DeserializeObject<Reply>(replyString) ??
               throw new InvalidOperationException("Could not deserialize reply");
    }
}