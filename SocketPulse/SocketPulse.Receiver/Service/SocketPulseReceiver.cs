using Newtonsoft.Json;
using SocketPulse.Receiver.CommandInvocation;
using SocketPulse.Receiver.Interfaces;
using SocketPulse.Receiver.Service.SocketWrapping;
using SocketPulse.Shared;

namespace SocketPulse.Receiver.Service;

public class SocketPulseReceiver : ISocketPulseReceiver
{
    private readonly ICommandInvoker _commandInvoker;
    private readonly IReceiverSocket _receiverSocket;

    public SocketPulseReceiver(ICommandInvoker commandInvoker, IReceiverSocket receiverSocket)
    {
        _commandInvoker = commandInvoker;
        _receiverSocket = receiverSocket;
    }

    public void Start(string address, CancellationToken cancellationToken, uint tickRateMs = 100)
    {
        SocketPulseReceiverSettings.TickRateMs = tickRateMs;
        _receiverSocket.InitSocket("@" + address);

        while (!cancellationToken.IsCancellationRequested)
        {
            var (senderIdentity, receivedMessage) = _receiverSocket.ReceiveFrameString();
            Reply result;
            try
            {
                result = HandleMessage(receivedMessage);
            }
            catch (Exception e)
            {
                result = new Reply { State = State.Error, Content = e.ToString() };
            }

            _receiverSocket.SendFrame(senderIdentity, JsonConvert.SerializeObject(result));
        }

        _receiverSocket.Close();
    }

    public void Stop()
    {
        _receiverSocket.Close();
    }

    public void Dispose()
    {
        Stop();
    }

    private Reply ExecuteAction(string function, Dictionary<string, string> arguments)
    {
        var action = _commandInvoker.GetCommand<IAction>(function);
        var result = action.Execute(arguments);
        return new Reply { State = result };
    }

    private Reply ExecuteCondition(string function, Dictionary<string, string> arguments)
    {
        var condition = _commandInvoker.GetCommand<ICondition>(function);
        var result = condition.Execute(arguments);
        return new Reply { State = result ? State.Success : State.Failure };
    }

    private Reply ExecuteDataNode(string function, Dictionary<string, string> arguments)
    {
        var data = _commandInvoker.GetCommand<IData>(function);
        var result = data.Execute(arguments);
        return new Reply { State = State.Success, Content = result };
    }

    private Reply HandleMessage(string message)
    {
        var request = JsonConvert.DeserializeObject<Request>(message);
        if (request == null) throw new InvalidOperationException("Invalid request");
        var argumentList = new Dictionary<string, string>();
        if (request.Arguments != null)
            argumentList = argumentList.Union(request.Arguments)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        var result = request.Type switch
        {
            RequestType.Action => ExecuteAction(request.Name, argumentList),
            RequestType.Condition => ExecuteCondition(request.Name, argumentList),
            RequestType.Data => ExecuteDataNode(request.Name, argumentList),
            _ => throw new InvalidOperationException($"Unsupported type: {request.Type}")
        };
        return result;
    }
}