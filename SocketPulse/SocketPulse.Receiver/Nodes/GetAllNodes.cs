using Newtonsoft.Json;
using SocketPulse.Receiver.CommandGeneration;
using SocketPulse.Receiver.Interfaces;
using SocketPulse.Shared;

namespace SocketPulse.Receiver.Nodes;

public class GetAllNodes : IData
{
    private readonly ICommandGenerator _commandGenerator;

    public GetAllNodes(ICommandGenerator commandGenerator)
    {
        _commandGenerator = commandGenerator;
    }

    public string Execute(List<string> arguments)
    {
        var data = new NodeInfo
        {
            Actions = _commandGenerator.GetActions(),
            Conditions = _commandGenerator.GetConditions(),
            Data = _commandGenerator.GetDataNodes()
        };
        return JsonConvert.SerializeObject(data, Formatting.Indented);
    }
}