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

    public uint GetTickRate()
    {
        var request = new Request { Type = RequestType.Data, Name = "GetTickRate" };
        var reply = SendRequest(request);
        return Convert.ToUInt32(reply.Content);
    }

    public NodeInfo GetAllNodes()
    {
        var request = new Request { Type = RequestType.Data, Name = "GetAllNodes" };
        var reply = SendRequest(request);
        return JsonConvert.DeserializeObject<NodeInfo>(reply.Content ??
                                                       throw new InvalidOperationException(
                                                           "Could not deserialize GetAllNodes")) ??
               throw new InvalidOperationException("Could not deserialize GetAllNodes");
    }
}