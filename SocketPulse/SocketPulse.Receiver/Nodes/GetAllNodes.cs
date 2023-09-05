using Newtonsoft.Json;
using SocketPulse.Receiver.CommandGeneration;
using SocketPulse.Receiver.Helpers;
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

    public string Execute(Dictionary<string, string> arguments)
    {
        var data = new NodeInfo
        {
            Actions = _commandGenerator.GetActions().Select(ClassNameExtractor.Extract).ToList(),
            Conditions = _commandGenerator.GetConditions().Select(ClassNameExtractor.Extract).ToList(),
            Data = _commandGenerator.GetDataNodes().Select(ClassNameExtractor.Extract).ToList(),
        };
        return JsonConvert.SerializeObject(data, Formatting.Indented);
    }
}