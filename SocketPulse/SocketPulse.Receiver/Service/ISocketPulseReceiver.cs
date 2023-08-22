namespace SocketPulse.Receiver.Service;

public interface ISocketPulseReceiver
{
    public void Start(string address, CancellationToken cancellationToken);
    public void Stop();
}