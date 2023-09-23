namespace SocketPulse.Receiver.Service;

public static class SocketPulseReceiverSettings
{
    public static uint TickRateMs { get; set; } = 100;
    public static Func<string> GetScreenBase64 { get; set; } = () => "";
    internal static List<Func<string>> GetStateInfo { get; } = new();
    public static void RegisterGetStateFunction(Func<string> getStateInfo)
    {
        GetStateInfo.Add(getStateInfo);
    }
}