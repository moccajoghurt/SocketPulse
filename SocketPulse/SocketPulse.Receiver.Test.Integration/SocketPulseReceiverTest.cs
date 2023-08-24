using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using SocketPulse.Receiver.Service;
using SocketPulse.Shared;
using SocketPulse.Test.Shared.Models;
using Xunit;

namespace SocketPulse.Receiver.Test.Integration;

public class SocketPulseReceiverTest
{
    private readonly ServiceCollection _serviceCollection = new();
    private readonly IServiceProvider _serviceProvider;

    public SocketPulseReceiverTest()
    {
        _serviceCollection.AddSocketPulseReceiver(new List<Assembly> { typeof(TestAction).Assembly });
        _serviceProvider = _serviceCollection.BuildServiceProvider();
    }

    [Theory]
    [InlineData(RequestType.Action, "TestAction", new[] { "arg1", "arg2", "arg3" })]
    [InlineData(RequestType.Condition, "TestCondition", new[] { "arg1", "arg2", "arg3" })]
    [InlineData(RequestType.Data, "TestData", new[] { "arg1", "arg2", "arg3" })]
    public void ActionSent_ShouldReturnSuccess(RequestType type, string functionName, string[] arguments)
    {
        // Arrange
        var service = _serviceProvider.GetService<ISocketPulseReceiver>();
        var cts = new CancellationTokenSource();
        Task.Run(() => service?.Start("tcp://*:8080", cts.Token), cts.Token);
        using var client = new RequestSocket("tcp://localhost:8080");
        var data = new Request
        {
            Type = type,
            Name = functionName,
            Arguments = new List<string>(arguments)
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

    [Fact]
    public void AllNodesRequested_ReturnsAllNodes()
    {
        // Arrange
        var service = _serviceProvider.GetService<ISocketPulseReceiver>();
        var cts = new CancellationTokenSource();
        Task.Run(() => service?.Start("tcp://*:8080", cts.Token), cts.Token);
        using var client = new RequestSocket("tcp://localhost:8080");
        var data = new Request
        {
            Type = RequestType.Data,
            Name = "GetAllNodes",
        };
        var message = JsonConvert.SerializeObject(data);

        // Act
        client.SendFrame(message);
        var replyStr = client.ReceiveFrameString();

        // Assert
        var reply = JsonConvert.DeserializeObject<Reply>(replyStr);
        Assert.NotNull(reply?.Content);
        var content = JsonConvert.DeserializeObject<NodeInfo>(reply.Content);
        Assert.NotNull(content);
        Assert.Contains(content.Actions, s => s.Contains("TestAction"));
        Assert.Contains(content.Conditions, s => s.Contains("TestCondition"));
        Assert.Contains(content.Data, s => s.Contains("TestData"));
        Assert.Contains(content.Data, s => s.Contains("GetAllNodes"));
        Assert.Contains(content.Data, s => s.Contains("GetTickRate"));
        service?.Stop();
    }
}