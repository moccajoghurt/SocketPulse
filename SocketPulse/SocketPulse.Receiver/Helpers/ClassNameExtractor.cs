namespace SocketPulse.Receiver.Helpers;

public static class ClassNameExtractor
{
    public static string Extract(string input)
    {
        var parts = input.Split(',');
        if (parts.Length < 2)
            return string.Empty;
        var subParts = parts[0].Split('.');
        return subParts[^1];
    }
}