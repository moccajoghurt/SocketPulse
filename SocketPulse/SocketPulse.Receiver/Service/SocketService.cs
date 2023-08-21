using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocketPulse.Receiver.CommandInvokation;
using SocketPulse.Receiver.Interfaces;
using SocketPulse.Receiver.Service.SocketWrapping;
using SocketPulse.Shared;

namespace SocketPulse.Receiver.Service;

public class SocketService : ISocketService
{
    private readonly ICommandInvoker _commandInvoker;
    private readonly IReceiverSocket _receiverSocket;

    public SocketService(ICommandInvoker commandInvoker, IReceiverSocket receiverSocket)
    {
        _commandInvoker = commandInvoker;
        _receiverSocket = receiverSocket;
    }

    public void Start(string address, CancellationToken cancellationToken)
    {

        _receiverSocket.InitSocket(address);

        while (!cancellationToken.IsCancellationRequested)
        {
            var message = _receiverSocket.ReceiveFrameString();
            var result = HandleMessage(message);
            _receiverSocket.SendFrame(JsonConvert.SerializeObject(result));
        }
        _receiverSocket.Close();
    }

    public void Stop()
    {
        _receiverSocket.Close();
    }

    private Reply ExecuteAction(string function, List<string> arguments)
    {
        var action = _commandInvoker.GetCommand<IAction>(function);
        var result = action.Execute(arguments);
        return new Reply { State = result };
    }

    private Reply ExecuteCondition(string function, List<string> arguments)
    {
        var condition = _commandInvoker.GetCommand<ICondition>(function);
        var result = condition.Execute(arguments);
        return new Reply { State = result ? State.Success : State.Failure };
    }

    private Reply ExecuteDataNode(string function, List<string> arguments)
    {
        var data = _commandInvoker.GetCommand<IData>(function);
        var result = data.Execute(arguments);
        return new Reply {State = State.Success, Content = result};
    }

    private Reply HandleMessage(string message)
    {
        var json = JObject.Parse(message);
        var typeName = json["typename"]!.ToString();
        var functionName = json["function"]!.ToString();
        var arguments = (JArray)json["arguments"]!;

        var result = typeName switch
        {
            "action" => ExecuteAction(functionName, arguments.Select(arg => arg.ToString()).ToList()),
            "condition" => ExecuteCondition(functionName, arguments.Select(arg => arg.ToString()).ToList()),
            "data" => ExecuteDataNode(functionName, arguments.Select(arg => arg.ToString()).ToList()),
            _ => throw new InvalidOperationException($"Unsupported type: {typeName}")
        };
        return result;
    }
}