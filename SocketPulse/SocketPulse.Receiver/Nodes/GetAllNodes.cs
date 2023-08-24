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
            Actions = _commandGenerator.GetActions().Select(ExtractName).ToList(),
            Conditions = _commandGenerator.GetConditions().Select(ExtractName).ToList(),
            Data = _commandGenerator.GetDataNodes().Select(ExtractName).ToList(),
        };
        return JsonConvert.SerializeObject(data, Formatting.Indented);
    }

    private string ExtractName(string input)
    {
        var parts = input.Split(',');
        if (parts.Length < 2)
            return string.Empty;
        var subParts = parts[0].Split('.');
        return subParts[^1];
    }
}