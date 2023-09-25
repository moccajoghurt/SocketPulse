namespace SocketPulse.Receiver.Service;

public interface ISocketPulseReceiver
{
    public void Start(string address, uint tickRateMs = 100);
    public void Stop();
}