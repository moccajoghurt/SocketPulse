using Microsoft.Extensions.DependencyInjection;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using SocketPulse.Receiver.Service;
using SocketPulse.Shared;
using Xunit;

namespace SocketPulse.Receiver.Test.Integration;

public class SocketServiceTest
{
    private readonly ServiceCollection _serviceCollection = new();
    private readonly IServiceProvider _serviceProvider;

    public SocketServiceTest()
    {
        _serviceCollection.AddSocketPulseReceiver(new[] { typeof(SocketServiceTest).Assembly });
        _serviceProvider = _serviceCollection.BuildServiceProvider();
    }

    [Theory]
    [InlineData("action", "TestAction", new[] { "arg1", "arg2", "arg3" })]
    [InlineData("condition", "TestCondition", new[] { "arg1", "arg2", "arg3" })]
    [InlineData("data", "TestData", new[] { "arg1", "arg2", "arg3" })]
    public void ActionSent_ShouldReturnSuccess(string typeName, string functionName, string[] arguments)
    {
        // Arrange
        var service = _serviceProvider.GetService<ISocketService>();
        var cts = new CancellationTokenSource();
        Task.Run(() => service?.Start("tcp://*:8080", cts.Token), cts.Token);
        using var client = new RequestSocket("tcp://localhost:8080");
        var data = new
        {
            typename = typeName,
            function = functionName,
            arguments
        };
        var message = JsonConvert.SerializeObject(data);

        // Act
        client.SendFrame(message);
        var replyStr = client.ReceiveFrameString();

        // Assert
        var reply = JsonConvert.DeserializeObject<Reply>(replyStr);
        Assert.Equal(State.Success, reply?.State);
        service?.Stop();
    }
}