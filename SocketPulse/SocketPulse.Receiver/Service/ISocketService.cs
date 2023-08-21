namespace SocketPulse.Receiver.Service;

public interface ISocketService
{
    public void Start(string address, CancellationToken cancellationToken);
    public void Stop();
}