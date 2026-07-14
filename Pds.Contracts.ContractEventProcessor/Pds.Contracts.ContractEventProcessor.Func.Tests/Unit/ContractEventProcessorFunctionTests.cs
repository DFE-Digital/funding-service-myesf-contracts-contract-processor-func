using FluentAssertions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Contracts.ContractEventProcessor.Services.Interfaces;
using Pds.Contracts.ContractEventProcessor.Services.Models;
using System;
using System.Threading.Tasks;

namespace Pds.Contracts.ContractEventProcessor.Func.Tests.Unit
{
    [TestClass]
    public class ContractEventProcessorFunctionTests
    {
        [TestMethod, TestCategory("Unit")]
        public void Run_DoesNotThrowException()
        {
            // Arrange
            var state = new SessionWorkflowState();
            var dummyMessage = new Message { MessageId = "1" };

            // Workaround to set internal get properties
            var systemProperties = new Message.SystemPropertiesCollection();
            typeof(Message.SystemPropertiesCollection).GetProperty(nameof(systemProperties.SequenceNumber)).SetValue(systemProperties, 1);
            typeof(Message.SystemPropertiesCollection).GetProperty(nameof(systemProperties.DeliveryCount)).SetValue(systemProperties, 1);
            typeof(Message.SystemPropertiesCollection).GetProperty(nameof(systemProperties.EnqueuedTimeUtc)).SetValue(systemProperties, DateTime.Now);
            typeof(Message.SystemPropertiesCollection).GetProperty(nameof(systemProperties.LockedUntilUtc)).SetValue(systemProperties, DateTime.Now);
            typeof(Message).GetProperty(nameof(dummyMessage.SystemProperties)).SetValue(dummyMessage, systemProperties);

            var mockSession = Mock.Of<IMessageSession>(MockBehavior.Strict);
            Mock.Get(mockSession)
                .SetupGet(s => s.SessionId)
                .Returns("session-1");

            var mockContractEventSessionManager = Mock.Of<IContractEventSessionManager>(MockBehavior.Strict);
            Mock.Get(mockContractEventSessionManager)
                .Setup(m => m.ProcessSessionMessageAsync(mockSession, dummyMessage))
                .ReturnsAsync(state)
                .Verifiable();

            var mockLogger = Mock.Of<ILogger>(MockBehavior.Strict);

            Mock.Get(mockLogger)
                .Setup(l => l.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null, It.IsAny<Func<It.IsAnyType, Exception, string>>()));

            Mock.Get(mockLogger)
                .Setup(l => l.BeginScope(It.IsAny<string>()))
                .Returns(It.IsAny<IDisposable>());

            var function = new ContractEventProcessorFunction(mockContractEventSessionManager);

            // Act
            Func<Task> act = async () => { await function.Run(dummyMessage, mockSession, mockLogger); };

            // Assert
            act.Should().NotThrow();
            Mock.Get(mockContractEventSessionManager).Verify();

            Mock.Get(mockLogger).VerifyAll();

            Mock.Get(mockSession)
                .VerifyGet(s => s.SessionId, Times.Exactly(3));
        }
    }
}