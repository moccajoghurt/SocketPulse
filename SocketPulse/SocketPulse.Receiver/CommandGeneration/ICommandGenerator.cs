namespace SocketPulse.Receiver.CommandGeneration;
public interface ICommandGenerator
{
    public List<string> GetConditions();
    public List<string> GetActions();
    public List<string> GetDataNodes();
}
