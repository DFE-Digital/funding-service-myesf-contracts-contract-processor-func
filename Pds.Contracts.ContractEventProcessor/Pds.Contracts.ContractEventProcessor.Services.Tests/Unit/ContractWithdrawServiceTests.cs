using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Contracts.ContractEventProcessor.Services;
using Pds.Contracts.ContractEventProcessor.Services.Enums;
using Pds.Contracts.ContractEventProcessor.Services.Implementations;
using Pds.Contracts.ContractEventProcessor.Services.Interfaces;
using Pds.Contracts.ContractEventProcessor.Services.Models;
using Pds.Contracts.Data.Api.Client.ConfigurationOptions;
using Pds.Contracts.Data.Api.Client.Implementations;
using Pds.Contracts.Data.Api.Client.Interfaces;
using Pds.Core.ApiClient.Interfaces;
using Pds.Core.Logging;
using RichardSzalay.MockHttp;
using System;
using System.Threading.Tasks;
using ClientData = Pds.Contracts.Data.Api.Client;

namespace Pds.Contracts.ContractEventProcessor.Services.Tests.Unit
{
    [TestClass, TestCategory("Unit")]
    public class ContractWithdrawServiceTests
    {
        private const string TestBaseAddress = "http://test-api-endpoint";

        private const string TestFakeAccessToken = "AccessToken";

        private readonly IContractEventProcessorLogger<IContractWithdrawService> _mockContractWithdrawServiceLogger
                = Mock.Of<IContractEventProcessorLogger<IContractWithdrawService>>(MockBehavior.Strict);

        private readonly ILoggerAdapter<ContractsDataService> _contractsDataLogger
            = Mock.Of<ILoggerAdapter<ContractsDataService>>(MockBehavior.Strict);

        private readonly IContractWithdrawService _mockContractsWithdrawService
           = Mock.Of<IContractWithdrawService>(MockBehavior.Strict);

        private readonly MockHttpMessageHandler _mockHttpMessageHandler
            = new MockHttpMessageHandler();

        private readonly IContractsDataService _mockContractsDataService
           = Mock.Of<IContractsDataService>(MockBehavior.Strict);

        [TestMethod]
        [DataRow(ContractParentStatus.Withdrawn, ContractStatus.WithdrawnByAgency, ContractAmendmentType.None)]
        [DataRow(ContractParentStatus.Withdrawn, ContractStatus.WithdrawnByAgency, ContractAmendmentType.Variation)]
        [DataRow(ContractParentStatus.Withdrawn, ContractStatus.WithdrawnByProvider, ContractAmendmentType.None)]
        [DataRow(ContractParentStatus.Withdrawn, ContractStatus.WithdrawnByProvider, ContractAmendmentType.Variation)]
        public void Withdraw_ExpectedResult_Test(ContractParentStatus parentStatus, ContractStatus contractStatus, ContractAmendmentType amendmentType)
        {
            //Arrange
            MockContractsDataService_WithdrawAsync();
            var contractEvent = GetContractEvent(parentStatus, contractStatus, amendmentType);
            var contract = GetClientContract();
            contract.Status = ClientData.Enumerations.ContractStatus.ApprovedWaitingConfirmation;
            var service = new ContractWithdrawService(_mockContractWithdrawServiceLogger, _mockContractsDataService);

            //Act
            service.WithdrawAsync(contractEvent).GetAwaiter().GetResult();

            //Assert
            Mock.Get(_mockContractsDataService)
              .Verify(d => d.WithdrawAsync(It.IsAny<ClientData.Models.WithdrawalRequest>()), Times.Once);
            Mock.Get(_mockContractWithdrawServiceLogger).VerifyAll();
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

        private void MockContractsDataService_WithdrawAsync()
        {
            Mock.Get(_mockContractsDataService)
                .Setup(x => x.WithdrawAsync(It.IsAny<ClientData.Models.WithdrawalRequest>()))
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
    }
}