using FluentAssertions;
using Microsoft.Azure.ServiceBus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Pds.Contracts.ContractEventProcessor.Services.Implementations;
using Pds.Contracts.ContractEventProcessor.Services.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Pds.Contracts.ContractEventProcessor.Services.Tests.Unit
{
    [TestClass, TestCategory("Unit")]
    public class WorkflowStateManagerTests
    {
        [TestMethod]
        public async Task GetWorkflowStateAsync_ReturnsExpectedResultTest()
        {
            // Arrange
            var expected = new SessionWorkflowState { FailedMessageId = "test" };
            var stateBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(expected));
            var mockSession = Mock.Of<IMessageSession>(MockBehavior.Strict);
            Mock.Get(mockSession)
                .Setup(s => s.GetStateAsync())
                .ReturnsAsync(stateBytes)
                .Verifiable();

            var workflowStateManager = new WorkflowStateManager();

            // Act
            var actual = await workflowStateManager.GetWorkflowStateAsync(mockSession);

            // Assert
            actual.Should().BeEquivalentTo(expected);
            Mock.Get(mockSession).Verify();
        }

        [TestMethod]
        public async Task GetWorkflowStateAsync_ReturnsEmptySessionStateTest()
        {
            // Arrange
            var expected = new SessionWorkflowState();
            var mockSession = Mock.Of<IMessageSession>(MockBehavior.Strict);
            Mock.Get(mockSession)
                .Setup(s => s.GetStateAsync())
                .ReturnsAsync(default(byte[]))
                .Verifiable();

            var workflowStateManager = new WorkflowStateManager();

            // Act
            var actual = await workflowStateManager.GetWorkflowStateAsync(mockSession);

            // Assert
            actual.Should().BeEquivalentTo(expected);
            Mock.Get(mockSession).Verify();
        }

        [TestMethod]
        public async Task ResetWorkflowStateAsync_ReturnsExpectedSessionStateTest()
        {
            // Arrange
            var expected = new SessionWorkflowState();
            var mockSession = Mock.Of<IMessageSession>(MockBehavior.Strict);
            Mock.Get(mockSession)
                .Setup(s => s.SetStateAsync(null))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var workflowStateManager = new WorkflowStateManager();

            // Act
            var actual = await workflowStateManager.ResetWorkflowStateAsync(mockSession);

            // Assert
            actual.Should().BeEquivalentTo(expected);
            Mock.Get(mockSession).Verify();
        }

        [TestMethod]
        public async Task SetWorkflowStateAsync_SetsExpectedSessionStateTest()
        {
            // Arrange
            var state = new SessionWorkflowState { FailedMessageId = "test" };
            var expected = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(state));
            var mockSession = Mock.Of<IMessageSession>(MockBehavior.Strict);
            Mock.Get(mockSession)
                .Setup(s => s.SetStateAsync(expected))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var workflowStateManager = new WorkflowStateManager();

            // Act
            await workflowStateManager.SetWorkflowStateAsync(mockSession, state);

            // Assert
            Mock.Get(mockSession).Verify();
        }
    }
}