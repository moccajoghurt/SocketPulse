namespace SocketPulse.Receiver.Interfaces;
public interface ICondition
{
    public bool Execute(Dictionary<string, string> arguments);
}
