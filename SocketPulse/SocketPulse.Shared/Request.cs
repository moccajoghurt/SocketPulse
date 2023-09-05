namespace SocketPulse.Shared;

public class Request
{
    public RequestType Type { get; set; }
    public string Name { get; set; } = null!;
    public Dictionary<string, string>? Arguments { get; set; }
}