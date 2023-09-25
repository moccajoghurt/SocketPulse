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

public class SocketPulseReceiverTest : IDisposable
{
    private readonly ServiceCollection _serviceCollection = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly CancellationTokenSource _cts;

    public SocketPulseReceiverTest()
    {
        _serviceCollection.AddSocketPulseReceiver(new List<Assembly> { typeof(TestAction).Assembly });
        _serviceProvider = _serviceCollection.BuildServiceProvider();
        _cts = new CancellationTokenSource();
    }

    [Theory]
    [InlineData(RequestType.Action, "TestAction", new[] { "arg1", "arg2" })]
    [InlineData(RequestType.Condition, "TestCondition", new[] { "arg1", "arg2" })]
    [InlineData(RequestType.Data, "TestData", new[] { "arg1", "arg2" })]
    public void ActionSent_ShouldReturnSuccess(RequestType type, string functionName, string[] arguments)
    {
        // Arrange
        var service = _serviceProvider.GetService<ISocketPulseReceiver>();
        Task.Run(() => service?.Start("tcp://*:8080", _cts.Token), _cts.Token);
        using var client = new RequestSocket("tcp://localhost:8080");
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
        Task.Run(() => service?.Start("tcp://*:8080", _cts.Token), _cts.Token);
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

        Task.Run(() =>
        {
            service?.Start("tcp://localhost:8080", _cts.Token);
        });
        Task.Delay(200).Wait();
        using var client = new DealerSocket("tcp://localhost:8080");
        // var message = JsonConvert.SerializeObject();

        // Act
        client.SendFrame("invalid data");
        var received = client.TryReceiveFrameString(TimeSpan.FromSeconds(2), out var replyStr);
        if (!received)
        {
            Assert.False(true, "Did not receive a reply within the expected time frame.");
        }

        // Assert
        var reply = JsonConvert.DeserializeObject<Reply>(replyStr!);
        Assert.Equal(State.Error, reply?.State);
        NetMQConfig.Cleanup();
    }

    public void Dispose()
    {
        // Release services
        var disposableService = _serviceProvider.GetService<ISocketPulseReceiver>() as IDisposable;
        disposableService?.Dispose();
        _cts.Cancel();
        _cts.Dispose();
        Task.Delay(200).Wait();
    }
}