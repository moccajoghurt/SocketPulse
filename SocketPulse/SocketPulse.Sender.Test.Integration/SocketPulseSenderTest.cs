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
        receiver?.Start("tcp://*:8080");
        var sender = SocketPulseSenderFactory.CreateSenderSocket();

        // Act
        var result = sender.Connect("tcp://localhost:8080");

        // Assert
        Assert.True(result);
        receiver?.Stop();
        sender.Close();
    }

    [Theory]
    [InlineData(RequestType.Action, "TestAction", new[] { "arg1", "arg2" })]
    [InlineData(RequestType.Action, "TestAction", new[] { "arg1", "arg2" }, 1000)]
    [InlineData(RequestType.Condition, "TestCondition", new[] { "arg1", "arg2" })]
    [InlineData(RequestType.Condition, "TestCondition", new[] { "arg1", "arg2" }, 1000)]
    [InlineData(RequestType.Data, "TestData", new[] { "arg1", "arg2" })]
    [InlineData(RequestType.Data, "TestData", new[] { "arg1", "arg2" }, 1000)]
    public void RequestSent_ShouldSucceed(RequestType type, string name, string[] arguments, int timeoutMs = 0)
    {
        // Arrange
        var receiver = _serviceProvider.GetService<ISocketPulseReceiver>();
        receiver?.Start("tcp://*:8080");
        var sender = SocketPulseSenderFactory.CreateSenderSocket();
        sender.Connect("tcp://localhost:8080");

        // Act
        var request = new Request
        {
            Type = type,
            Name = name,
            Arguments = new Dictionary<string, string> { { "arg1", arguments[0] }, { "arg2", arguments[1] } }
        };
        var reply = timeoutMs == 0
            ? sender.SendRequest(request)
            : sender.SendRequest(request, TimeSpan.FromMilliseconds(timeoutMs));

        // Assert
        Assert.Equal(State.Success, reply.State);
        receiver?.Stop();
        sender.Close();
    }

    [Fact]
    public void GetAllNodes_NodesReturns()
    {
        // Arrange
        var receiver = _serviceProvider.GetService<ISocketPulseReceiver>();
        receiver?.Start("tcp://*:8080");
        var sender = SocketPulseSenderFactory.CreateSenderSocket();
        sender.Connect("tcp://localhost:8080");

        // Act
        var nodes = sender.GetAllNodes();

        // Assert
        Assert.NotNull(nodes);
        Assert.True(nodes.Data.Any());
        Assert.True(nodes.Actions.Any());
        Assert.True(nodes.Conditions.Any());
        receiver?.Stop();
        sender.Close();
    }

    [Fact]
    public void GetTickRate_ReturnsTickRate()
    {
        // Arrange
        var myTickRate = 100u;
        var receiver = _serviceProvider.GetService<ISocketPulseReceiver>();
        receiver?.Start("tcp://*:8080");
        var sender = SocketPulseSenderFactory.CreateSenderSocket();
        sender.Connect("tcp://localhost:8080");

        // Act
        var tickRate = sender.GetTickRate();

        // Assert
        Assert.Equal(myTickRate, tickRate);
        receiver?.Stop();
        sender.Close();
    }
}