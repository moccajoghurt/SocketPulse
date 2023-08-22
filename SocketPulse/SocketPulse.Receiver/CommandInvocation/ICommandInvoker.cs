namespace SocketPulse.Receiver.CommandInvocation;
public interface ICommandInvoker
{
    public T GetCommand<T>(string name);
}
