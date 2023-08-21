using SocketPulse.Receiver.Interfaces;

namespace SocketPulse.Receiver.CommandGeneration;

public class CommandGenerator : ICommandGenerator
{
    public List<string> GetConditions()
    {
        return GetClassesByType(typeof(ICondition));
    }

    public List<string> GetActions()
    {
        return GetClassesByType(typeof(IAction));
    }

    public List<string> GetDataNodes()
    {
        return GetClassesByType(typeof(IData));
    }

    private static List<string> GetClassesByType(Type type)
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes().Where(type.IsAssignableFrom).Select(t => $"{t.FullName}, {s.GetName().Name}"))
            .ToList();
        return types;
    }
}