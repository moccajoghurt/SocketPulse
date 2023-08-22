using SocketPulse.Receiver.CommandGeneration;

namespace SocketPulse.Receiver.CommandInvocation;

public class CommandInvoker : ICommandInvoker
{
    private readonly List<string> _actions;
    private readonly List<string> _conditions;
    private readonly List<string> _dataNodes;
    private readonly IServiceProvider _serviceProvider;

    public CommandInvoker(IServiceProvider serviceProvider, ICommandGenerator commandGenerator)
    {
        _serviceProvider = serviceProvider;
        _conditions = commandGenerator.GetConditions();
        _actions = commandGenerator.GetActions();
        _dataNodes = commandGenerator.GetDataNodes();
    }

    public T GetCommand<T>(string name)
    {
        var commandName = _conditions.Find(s => s.Contains(name)) ??
                          _actions.Find(s => s.Contains(name)) ?? _dataNodes.Find(s => s.Contains(name));
        if (commandName == null) throw new InvalidOperationException($"{name} not found");
        var type = Type.GetType(commandName ?? throw new InvalidOperationException($"{name} not found"));
        var service = (T)_serviceProvider.GetService(type ?? throw new InvalidOperationException($"{name} not found"))!;
        return service;
    }
}