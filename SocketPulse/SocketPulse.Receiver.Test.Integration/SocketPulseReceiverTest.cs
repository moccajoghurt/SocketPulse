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
    [InlineData(RequestType.Action, "TestAction", new[] { "arg1", "arg2" })]
    [InlineData(RequestType.Condition, "TestCondition", new[] { "arg1", "arg2" })]
    [InlineData(RequestType.Data, "TestData", new[] { "arg1", "arg2" })]
    public void ActionSent_ShouldReturnSuccess(RequestType type, string functionName, string[] arguments)
    {
        // Arrange
        var service = _serviceProvider.GetService<ISocketPulseReceiver>();
        service?.Start("tcp://*:8080");
        using var client = new DealerSocket("tcp://localhost:8080");
        var data = new Request
        {
            Type = type,
            Name = functionName,
            Arguments = new Dictionary<string, string> { { "arg1", arguments[0] }, { "arg2", arguments[1] } }
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
        service?.Start("tcp://*:8080");
        using var client = new DealerSocket("tcp://localhost:8080");
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

    [Fact]
    public void InvalidRequest_ReturnsErrorReply()
    {
        // Arrange
        var service = _serviceProvider.GetService<ISocketPulseReceiver>();
        service?.Start("tcp://localhost:13337");
        using var dealer = new DealerSocket("tcp://localhost:13337");
        try
        {
            // Act
            dealer.SendFrame("invalid data");
            var success = dealer.TryReceiveFrameString(TimeSpan.FromSeconds(200), out var replyStr);

            // Assert
            Assert.True(success, "Timed out.");
            var reply = JsonConvert.DeserializeObject<Reply>(replyStr!);
            Assert.Equal(State.Error, reply?.State);
        }
        finally
        {
            service?.Stop();
            dealer.Close();
        }
    }
}