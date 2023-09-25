namespace SocketPulse.Receiver.Service;

public interface ISocketPulseReceiver : IDisposable
{
    public void Start(string address, CancellationToken cancellationToken, uint tickRateMs = 100);
    public void Stop();
}