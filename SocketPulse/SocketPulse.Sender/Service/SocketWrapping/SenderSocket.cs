using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using SocketPulse.Shared;

namespace SocketPulse.Sender.Service.SocketWrapping;

public class SenderSocket : ISenderSocket
{
    private readonly DealerSocket _dealerSocket = new();

    public bool Connect(string address)
    {
        _dealerSocket.Connect(address);
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

    public Reply SendRequest(Request request, TimeSpan? timeout = null)
    {
        var requestString = JsonConvert.SerializeObject(request);
        if (timeout == null)
        {
            _dealerSocket.SendFrame(requestString);
        }
        else
        {
            var success = _dealerSocket.TrySendFrame((TimeSpan)timeout, requestString);
            if (!success) throw new TimeoutException("Request timed out");
        }

        string replyString;
        if (timeout == null)
            replyString = _dealerSocket.ReceiveFrameString();
        else
            replyString = _dealerSocket.TryReceiveFrameString(timeout.Value, out var result)
                ? result
                : throw new TimeoutException("Request timed out");
        var reply = JsonConvert.DeserializeObject<Reply>(replyString) ??
                    throw new InvalidOperationException("Could not deserialize reply");
        if (reply.State == State.Error)
            Console.WriteLine("SocketPulseSender: Exception occurred on remote machine:\n" + reply.Content);
        return reply;
    }

    public void Close()
    {
        _dealerSocket.Close();
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