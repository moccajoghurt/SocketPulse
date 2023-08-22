namespace SocketPulse.Shared;

public class Request
{
    public string Type { get; set; } = null!;
    public string Name { get; set; } = null!;
    public List<string>? Arguments { get; set; }
}