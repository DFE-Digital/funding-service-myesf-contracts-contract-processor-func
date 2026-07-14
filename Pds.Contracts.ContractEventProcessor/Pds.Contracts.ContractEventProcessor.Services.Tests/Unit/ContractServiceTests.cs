using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Contracts.ContractEventProcessor.Services.CustomExceptionHandlers;
using Pds.Contracts.ContractEventProcessor.Services.Implementations;
using Pds.Contracts.ContractEventProcessor.Services.Interfaces;
using Pds.Contracts.ContractEventProcessor.Services.Models;
using Pds.Contracts.Data.Api.Client.Interfaces;
using Pds.Contracts.Data.Api.Client.Models;
using System;
using System.Threading.Tasks;
using ClientData = Pds.Contracts.Data.Api.Client;

namespace Pds.Contracts.ContractEventProcessor.Services.Tests.Unit
{
    [TestClass, TestCategory("Unit")]
    public class ContractServiceTests
    {
        private readonly IContractCreationService _mockContractCreationService
            = Mock.Of<IContractCreationService>(MockBehavior.Strict);

        private readonly IContractEventProcessorLogger<IContractService> _mockContractServiceLogger
                = Mock.Of<IContractEventProcessorLogger<IContractService>>(MockBehavior.Strict);

        private readonly ILogger<IContractApprovalService> _mockContractApprovalServiceLogger
                = Mock.Of<ILogger<IContractApprovalService>>(MockBehavior.Strict);

        private readonly ILogger<IContractWithdrawService> _mockContractWithdrawServiceLogger
                = Mock.Of<ILogger<IContractWithdrawService>>(MockBehavior.Strict);

        private readonly IContractService _mockContractsService
           = Mock.Of<IContractService>(MockBehavior.Strict);

        private readonly IContractApprovalService _mockContractApprovalService
           = Mock.Of<IContractApprovalService>(MockBehavior.Strict);

        private readonly IContractWithdrawService _mockContractsWithdrawService
           = Mock.Of<IContractWithdrawService>(MockBehavior.Strict);

        private readonly IContractsDataService _mockContractsDataService
           = Mock.Of<IContractsDataService>(MockBehavior.Strict);

        [TestMethod]
        [DataRow(ContractParentStatus.Approved, ContractStatus.Approved, ContractAmendmentType.None)]
        public void ProcessMessage_Approved_ReturnsExpectedResult(ContractParentStatus parentStatus, ContractStatus contractStatus, ContractAmendmentType amendmentType)
        {
            // Arrange
            Mock.Get(_mockContractServiceLogger)
                .Setup(l => l.LogInformation(It.IsAny<string>()));

            var dummyContractEvent = GetContractEvent(parentStatus, contractStatus, amendmentType);
            ClientData.Models.Contract dummyContract = GetClientContract();

            Mock.Get(_mockContractApprovalService)
                .Setup(s => s.ApproveAsync(It.IsAny<ContractEvent>(), It.IsAny<Contract>()))
                .ReturnsAsync(true)
                .Verifiable();

            Mock.Get(_mockContractsDataService)
               .Setup(s => s.TryGetContractAsync(It.IsAny<string>(), It.IsAny<int>()))
               .ReturnsAsync(dummyContract)
               .Verifiable();

            var contractService = new ContractService(_mockContractServiceLogger, _mockContractsDataService, _mockContractApprovalService, _mockContractsWithdrawService, _mockContractCreationService);

            // Act
            contractService.ProcessMessage(dummyContractEvent).GetAwaiter().GetResult();

            // Assert
            Mock.Get(_mockContractsDataService)
               .Verify(s => s.TryGetContractAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            Mock.Get(_mockContractServiceLogger).VerifyAll();
        }

        [TestMethod]
        [DataRow(ContractParentStatus.Approved, ContractStatus.Approved, ContractAmendmentType.None)]
        public void ProcessMessage_Approved_ReturnsLogAndExitResult(ContractParentStatus parentStatus, ContractStatus contractStatus, ContractAmendmentType amendmentType)
        {
            // Arrange
            var dummyContractEvent = GetContractEvent(parentStatus, contractStatus, amendmentType);
            var dummyContract = default(Contract);

            Mock.Get(_mockContractServiceLogger)
                .Setup(l => l.LogInformation(It.IsAny<string>()));
            Mock.Get(_mockContractServiceLogger)
                .Setup(l => l.LogWarning(It.IsAny<string>()));

            Mock.Get(_mockContractApprovalService)
                .Setup(s => s.ApproveAsync(It.IsAny<ContractEvent>(), It.IsAny<Contract>()))
                .ReturnsAsync(true)
                .Verifiable();

            Mock.Get(_mockContractsDataService)
               .Setup(s => s.TryGetContractAsync(It.IsAny<string>(), It.IsAny<int>()))
               .ReturnsAsync(dummyContract)
               .Verifiable();

            var contractService = new ContractService(_mockContractServiceLogger, _mockContractsDataService, _mockContractApprovalService, _mockContractsWithdrawService, _mockContractCreationService);

            // Act
            contractService.ProcessMessage(dummyContractEvent).GetAwaiter().GetResult();

            // Assert
            Mock.Get(_mockContractsDataService)
               .Verify(s => s.TryGetContractAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            Mock.Get(_mockContractApprovalService)
                .Verify(s => s.ApproveAsync(It.IsAny<ContractEvent>(), It.IsAny<Contract>()), Times.Never);
            Mock.Get(_mockContractServiceLogger).VerifyAll();
        }

        [TestMethod]
        [DataRow(ContractParentStatus.Approved, ContractStatus.Replaced, ContractAmendmentType.None)]
        public void ProcessMessage_ThrowsException(ContractParentStatus parentStatus, ContractStatus contractStatus, ContractAmendmentType amendmentType)
        {
            // Arrange
            var dummyContractEvent = GetContractEvent(parentStatus, contractStatus, amendmentType);
            Mock.Get(_mockContractServiceLogger)
                .Setup(l => l.LogInformation(It.IsAny<string>()));

            Mock.Get(_mockContractApprovalService)
                .Setup(s => s.ApproveAsync(It.IsAny<ContractEvent>(), It.IsAny<Contract>()))
                .Returns(It.IsAny<Task<bool>>())
                .Verifiable();

            Mock.Get(_mockContractsWithdrawService)
                .Setup(s => s.WithdrawAsync(It.IsAny<ContractEvent>()))
                .Returns(It.IsAny<Task<bool>>())
                .Verifiable();

            // Act
            var contractService = new ContractService(_mockContractServiceLogger, _mockContractsDataService, _mockContractApprovalService, _mockContractsWithdrawService, _mockContractCreationService);
            Func<Task> act = async () => await contractService.ProcessMessage(dummyContractEvent);

            // Assert
            act.Should().Throw<NotImplementedException>();
            Mock.Get(_mockContractServiceLogger).VerifyAll();
        }

        [TestMethod]
        [DataRow(ContractParentStatus.Withdrawn, ContractStatus.WithdrawnByAgency, ContractAmendmentType.None)]
        [DataRow(ContractParentStatus.Withdrawn, ContractStatus.WithdrawnByAgency, ContractAmendmentType.Variation)]
        [DataRow(ContractParentStatus.Withdrawn, ContractStatus.WithdrawnByProvider, ContractAmendmentType.None)]
        [DataRow(ContractParentStatus.Withdrawn, ContractStatus.WithdrawnByProvider, ContractAmendmentType.Variation)]
        public void ProcessMessage_WhenWithdrawEventType_RequestsWithdrawl(ContractParentStatus parentStatus, ContractStatus contractStatus, ContractAmendmentType amendmentType)
        {
            // Arrange
            var dummyContractEvent = GetContractEvent(parentStatus, contractStatus, amendmentType);

            Mock.Get(_mockContractServiceLogger)
                .Setup(l => l.LogInformation(It.IsAny<string>()));

            Mock.Get(_mockContractsWithdrawService)
                .Setup(s => s.WithdrawAsync(It.IsAny<ContractEvent>()))
                .ReturnsAsync(true)
                .Verifiable();

            var contractService = new ContractService(_mockContractServiceLogger, _mockContractsDataService, _mockContractApprovalService, _mockContractsWithdrawService, _mockContractCreationService);

            // Act
            Func<Task> act = async () => await contractService.ProcessMessage(dummyContractEvent);

            // Assert
            act.Should().NotThrow();

            Mock.Get(_mockContractsWithdrawService)
                .Verify(s => s.WithdrawAsync(It.IsAny<ContractEvent>()), Times.Once);
            Mock.Get(_mockContractServiceLogger).VerifyAll();
            Mock.Get(_mockContractApprovalService).VerifyAll();
            Mock.Get(_mockContractsDataService).VerifyAll();
        }

        private void MockContractsDataService_GetContractAsync(ClientData.Models.Contract contract)
        {
            Mock.Get(_mockContractsDataService)
                .Setup(x => x.GetContractAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(contract)
                .Verifiable();
        }

        private ContractEvent GetContractEvent(ContractParentStatus parentStatus, ContractStatus contractStatus, ContractAmendmentType amendmentType)
        {
            return new ContractEvent()
            {
                ParentStatus = parentStatus,
                Status = contractStatus,
                AmendmentType = amendmentType,
                ContractNumber = "ABC-0000",
                ContractVersion = 1,
                ContractEventXml = "sample-blob-file.xml"
            };
        }

        private ClientData.Models.Contract GetClientContract()
        {
            return new ClientData.Models.Contract()
            {
                Id = 1,
                ContractNumber = "ABC-0000",
                ContractVersion = 1
            };
        }
    }
}