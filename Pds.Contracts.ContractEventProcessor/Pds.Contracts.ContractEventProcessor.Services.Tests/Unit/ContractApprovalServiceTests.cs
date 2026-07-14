using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Contracts.ContractEventProcessor.Services.Enums;
using Pds.Contracts.ContractEventProcessor.Services.Implementations;
using Pds.Contracts.ContractEventProcessor.Services.Interfaces;
using Pds.Contracts.ContractEventProcessor.Services.Models;
using Pds.Contracts.Data.Api.Client.Implementations;
using Pds.Contracts.Data.Api.Client.Interfaces;
using Pds.Core.Logging;
using System;
using System.Threading.Tasks;
using ClientData = Pds.Contracts.Data.Api.Client;

namespace Pds.Contracts.ContractEventProcessor.Services.Tests.Unit
{
    [TestClass, TestCategory("Unit")]
    public class ContractApprovalServiceTests
    {
        private const string TestBaseAddress = "http://test-api-endpoint";

        private const string TestFakeAccessToken = "AccessToken";

        private readonly IContractEventProcessorLogger<IContractApprovalService> _mockContractApprovalServiceLogger
                = Mock.Of<IContractEventProcessorLogger<IContractApprovalService>>(MockBehavior.Strict);

        private readonly ILoggerAdapter<ContractsDataService> _contractsDataLogger
            = Mock.Of<ILoggerAdapter<ContractsDataService>>(MockBehavior.Strict);

        private readonly IContractApprovalService _mockContractsApprovalService
           = Mock.Of<IContractApprovalService>(MockBehavior.Strict);


        private readonly IContractsDataService _mockContractsDataService
           = Mock.Of<IContractsDataService>(MockBehavior.Strict);

        [TestMethod]
        [DataRow(ContractParentStatus.Approved, ContractStatus.Approved, ContractAmendmentType.None)]
        [DataRow(ContractParentStatus.Approved, ContractStatus.Approved, ContractAmendmentType.Variation)]
        [DataRow(ContractParentStatus.Approved, ContractStatus.Modified, ContractAmendmentType.None)]
        [DataRow(ContractParentStatus.Approved, ContractStatus.Modified, ContractAmendmentType.Variation)]
        [DataRow(ContractParentStatus.Approved, ContractStatus.UnderTermination, ContractAmendmentType.None)]
        [DataRow(ContractParentStatus.Approved, ContractStatus.UnderTermination, ContractAmendmentType.Variation)]
        public void ConfirmApprove_ExpectedResult_Test(ContractParentStatus parentStatus, ContractStatus contractStatus, ContractAmendmentType amendmentType)
        {
            //Arrange
            MockContractsDataService_ConfirmApprovalAsync();

            var contractEvent = GetContractEvent(parentStatus, contractStatus, amendmentType);
            var contract = GetClientContract();
            contract.Status = ClientData.Enumerations.ContractStatus.ApprovedWaitingConfirmation;
            var service = new ContractApprovalService(_mockContractApprovalServiceLogger, _mockContractsDataService);

            //Act
            service.ApproveAsync(contractEvent, contract).GetAwaiter().GetResult();

            //Assert
            Mock.Get(_mockContractsDataService)
              .Verify(d => d.ConfirmApprovalAsync(It.IsAny<ClientData.Models.ApprovalRequest>()), Times.Once);
            Mock.Get(_mockContractApprovalServiceLogger).VerifyAll();
        }

        [TestMethod]
        [DataRow(ContractParentStatus.Approved, ContractStatus.Approved, ContractAmendmentType.None)]
        [DataRow(ContractParentStatus.Approved, ContractStatus.Approved, ContractAmendmentType.Variation)]
        [DataRow(ContractParentStatus.Approved, ContractStatus.Modified, ContractAmendmentType.None)]
        [DataRow(ContractParentStatus.Approved, ContractStatus.Modified, ContractAmendmentType.Variation)]
        [DataRow(ContractParentStatus.Approved, ContractStatus.UnderTermination, ContractAmendmentType.None)]
        [DataRow(ContractParentStatus.Approved, ContractStatus.UnderTermination, ContractAmendmentType.Variation)]
        public void ManualApprove_ExpectedResult_Test(ContractParentStatus parentStatus, ContractStatus contractStatus, ContractAmendmentType amendmentType)
        {
            // Arrange
            MockContractsDataService_ConfirmApprovalAsync();
            MockContractsDataService_ManualApprovalAsync();

            var contractEvent = GetContractEvent(parentStatus, contractStatus, amendmentType);
            var contract = GetClientContract();
            contract.Status = ClientData.Enumerations.ContractStatus.PublishedToProvider;
            var service = new ContractApprovalService(_mockContractApprovalServiceLogger, _mockContractsDataService);

            // Act
            service.ApproveAsync(contractEvent, contract).GetAwaiter().GetResult();

            // Assert
            Mock.Get(_mockContractApprovalServiceLogger).VerifyAll();
        }

        private void MockContractsDataService_ILoggerAdapter()
        {
            Mock.Get(_contractsDataLogger)
                .Setup(p => p.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()));
        }

        private void MockContractsDataService_GetContractAsync(ClientData.Models.Contract contract)
        {
            Mock.Get(_mockContractsDataService)
                .Setup(x => x.GetContractAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(contract)
                .Verifiable();
        }

        private void MockContractsDataService_GetContractAsync_AnyReturn()
        {
            Mock.Get(_mockContractsDataService)
                .Setup(x => x.GetContractAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(It.IsAny<ClientData.Models.Contract>())
                .Verifiable();
        }

        private void MockContractsDataService_ConfirmApprovalAsync()
        {
            Mock.Get(_mockContractsDataService)
                .Setup(x => x.ConfirmApprovalAsync(It.IsAny<ClientData.Models.ApprovalRequest>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
        }

        private void MockContractsDataService_ManualApprovalAsync()
        {
            Mock.Get(_mockContractsDataService)
                .Setup(x => x.ManualApproveAsync(It.IsAny<ClientData.Models.ApprovalRequest>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
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

        private void SetupLogger_LogInformation()
        {
            Mock.Get(_mockContractApprovalServiceLogger)
               .Setup(logger => logger.LogInformation(It.IsAny<string>()));
        }

        private void SetupLogger_LogWarning()
        {
            Mock.Get(_mockContractApprovalServiceLogger)
               .Setup(logger => logger.LogWarning(It.IsAny<string>()));
        }

        private void SetupLogger_LogError<TException>()
            where TException : Exception
        {
            Mock.Get(_mockContractApprovalServiceLogger)
               .Setup(logger => logger.LogError(It.Is<TException>(e => e.Equals(default(TException))), It.IsAny<string>()));
        }
    }
}