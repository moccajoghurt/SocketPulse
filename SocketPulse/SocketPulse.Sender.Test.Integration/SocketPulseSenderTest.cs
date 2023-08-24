using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SocketPulse.Receiver;
using SocketPulse.Receiver.Service;
using SocketPulse.Sender.Service;
using SocketPulse.Shared;
using SocketPulse.Test.Shared.Models;
using Xunit;

namespace SocketPulse.Sender.Test.Integration;

public class SocketPulseSenderTest
{
    private readonly ServiceCollection _serviceCollection = new();
    private readonly IServiceProvider _serviceProvider;

    public SocketPulseSenderTest()
    {
        _serviceCollection.AddSocketPulseReceiver(new List<Assembly> { typeof(TestAction).Assembly });
        _serviceCollection.AddSocketPulseSender();
        _serviceProvider = _serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public void InitConnection_PingPongSucceeds()
    {
        // Arrange
        var receiver = _serviceProvider.GetService<ISocketPulseReceiver>();
        var cts = new CancellationTokenSource();
        Task.Run(() => receiver?.Start("tcp://*:8080", cts.Token), cts.Token);
        var sender = _serviceProvider.GetService<ISocketPulseSender>();

        // Act
        var result = sender?.Connect("tcp://localhost:8080");

        // Assert
        Assert.True(result);
        receiver?.Stop();
        sender?.Disconnect();
    }

    [Theory]
    [InlineData(RequestType.Action, "TestAction", new[] { "arg1", "arg2", "arg3" })]
    [InlineData(RequestType.Condition, "TestCondition", new[] { "arg1", "arg2", "arg3" })]
    [InlineData(RequestType.Data, "TestData", new[] { "arg1", "arg2", "arg3" })]
    public void RequestSent_ShouldSucceed(RequestType type, string name, string[] arguments)
    {
        // Arrange
        var receiver = _serviceProvider.GetService<ISocketPulseReceiver>();
        var cts = new CancellationTokenSource();
        Task.Run(() => receiver?.Start("tcp://*:8080", cts.Token), cts.Token);
        var sender = _serviceProvider.GetService<ISocketPulseSender>();
        sender?.Connect("tcp://localhost:8080");

        // Act
        var request = new Request
        {
            Type = type,
            Name = name,
            Arguments = arguments.ToList()
        };
        var reply = sender?.SendRequest(request);

        // Assert
        Assert.Equal(State.Success, reply?.State);
        receiver?.Stop();
        sender?.Disconnect();
    }

    [Fact]
    public void GetAllNodes_NodesReturns()
    {
        // Arrange
        var receiver = _serviceProvider.GetService<ISocketPulseReceiver>();
        var cts = new CancellationTokenSource();
        Task.Run(() => receiver?.Start("tcp://*:8080", cts.Token), cts.Token);
        var sender = _serviceProvider.GetService<ISocketPulseSender>();
        sender?.Connect("tcp://localhost:8080");

        // Act
        var nodes = sender?.GetAllNodes();

        // Assert
        Assert.NotNull(nodes);
        Assert.True(nodes.Data.Any());
        Assert.True(nodes.Actions.Any());
        Assert.True(nodes.Conditions.Any());
        receiver?.Stop();
        sender?.Disconnect();
    }

    [Fact]
    public void GetTickRate_ReturnsTickRate()
    {
        // Arrange
        var myTickRate = 150u;
        var receiver = _serviceProvider.GetService<ISocketPulseReceiver>();
        var cts = new CancellationTokenSource();
        Task.Run(() => receiver?.Start("tcp://*:8080", cts.Token, myTickRate), cts.Token);
        var sender = _serviceProvider.GetService<ISocketPulseSender>();
        sender?.Connect("tcp://localhost:8080");

        // Act
        var tickRate = sender?.GetTickRate();

        // Assert
        Assert.NotNull(tickRate);
        Assert.Equal(myTickRate, tickRate);
        receiver?.Stop();
        sender?.Disconnect();
    }
}