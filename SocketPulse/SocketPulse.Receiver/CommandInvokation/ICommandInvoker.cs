namespace SocketPulse.Receiver.CommandInvokation;
public interface ICommandInvoker
{
    public T GetCommand<T>(string name);
}
