using SocketPulse.Sender.Service.SocketWrapping;

namespace SocketPulse.Sender.Service;

public static class SocketPulseSenderFactory
{
    public static ISenderSocket CreateSenderSocket()
    {
        return new SenderSocket();
    }
}