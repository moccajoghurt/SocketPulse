using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using SocketPulse.Receiver.CommandInvocation;
using SocketPulse.Receiver.Interfaces;
using SocketPulse.Shared;

namespace SocketPulse.Receiver.Service;

public class SocketPulseReceiver : ISocketPulseReceiver
{
    private readonly ICommandInvoker _commandInvoker;

    private volatile bool _isRunning;
    private Thread? _thread;

    public SocketPulseReceiver(ICommandInvoker commandInvoker)
    {
        _commandInvoker = commandInvoker;
    }

    public void Start(string address, uint tickRateMs = 100)
    {
        if (_isRunning) throw new InvalidOperationException("Already running");
        SocketPulseReceiverSettings.TickRateMs = tickRateMs;
        _isRunning = true;
        _thread = new Thread(() => Worker(address));
        _thread.Start();
    }

    public void Stop()
    {
        _isRunning = false;
        _thread?.Join(); // because of "using" the socket will be disposed
    }

    private void Worker(string address)
    {
        using var routerSocket = new RouterSocket();
        routerSocket.Bind(address);

        while (_isRunning)
        {
            var msg = new NetMQMessage();
            try
            {
                routerSocket.TryReceiveMultipartMessage(TimeSpan.FromMilliseconds(100), ref msg, 2);
            }
            catch (NetMQException)
            {
                // NetMQ internally uses exceptions that we don't worry about
            }

            if (msg == null || msg.FrameCount == 0) continue;
            if (msg.FrameCount != 2)
                throw new InvalidOperationException(
                    "Unexpected msg received. The dealer socket should send his identity and the message");
            var identity = msg.Pop();
            var content = msg.Pop().ConvertToString();
            Reply result;
            try
            {
                result = HandleMessage(content);
            }
            catch (Exception e)
            {
                result = new Reply { State = State.Error, Content = e.ToString() };
            }
            NetMQMessage reply = new();
            reply.Append(identity);
            reply.Append(JsonConvert.SerializeObject(result));
            routerSocket.TrySendMultipartMessage(reply);
        }
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