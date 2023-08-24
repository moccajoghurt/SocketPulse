namespace SocketPulse.Receiver.Service;

public interface ISocketPulseReceiver
{
    public void Start(string address, CancellationToken cancellationToken, uint tickRateMs = 100);
    public void Stop();
}