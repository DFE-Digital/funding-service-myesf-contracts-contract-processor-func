using Pds.Contracts.ContractEventProcessor.Services.Enums;
using Pds.Contracts.ContractEventProcessor.Services.Interfaces;
using Pds.Contracts.ContractEventProcessor.Services.Models;
using Pds.Contracts.Data.Api.Client.Interfaces;
using System;
using System.Threading.Tasks;

namespace Pds.Contracts.ContractEventProcessor.Services.Implementations
{
    /// <summary>
    /// Contract service.
    /// </summary>
    public class ContractService : IContractService
    {
        private readonly IContractEventProcessorLogger<IContractService> _logger;
        private readonly IContractsDataService _contractsDataService;
        private readonly IContractApprovalService _contractApprovalService;
        private readonly IContractWithdrawService _contractWithdrawService;
        private readonly IContractCreationService _contractCreationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="contractsDataService">The contracts data service.</param>
        /// <param name="contractApprovalService">The contract approval service.</param>
        /// <param name="validationService">The validation service.</param>
        /// <param name="contractWithdrawService">The contract withdraw service.</param>
        /// <param name="contractCreationService">The contract creation service.</param>
        public ContractService(
            IContractEventProcessorLogger<IContractService> logger,
            IContractsDataService contractsDataService,
            IContractApprovalService contractApprovalService,
            IContractWithdrawService contractWithdrawService,
            IContractCreationService contractCreationService)
        {
            _logger = logger;
            _contractsDataService = contractsDataService;
            _contractApprovalService = contractApprovalService;
            _contractWithdrawService = contractWithdrawService;
            _contractCreationService = contractCreationService;
        }

        /// <inheritdoc/>
        public async Task ProcessMessage(ContractEvent contractEvent)
        {
            _logger.LogInformation($"[{nameof(ProcessMessage)}] Processing message for contract event : {contractEvent.BookmarkId}");

            var eventType = contractEvent.GetContractEventType();
            switch (eventType)
            {
                case ContractEventType.Create:
                    var existingContract = await _contractsDataService.TryGetContractAsync(contractEvent.ContractNumber, contractEvent.ContractVersion);
                    if (existingContract is null)
                    {
                        await _contractCreationService.CreateAsync(contractEvent);
                    }
                    else
                    {
                        _logger.LogWarning($"[{nameof(ContractEventProcessor)}] - Ignoring contract event with id [{contractEvent.BookmarkId}] because a contract with contract number [{contractEvent.ContractNumber}], version [{contractEvent.ContractVersion}] and Id [{existingContract.Id}] already exists.");
                    }

                    break;

                case ContractEventType.Approve:
                    var approveContract = await _contractsDataService.TryGetContractAsync(contractEvent.ContractNumber, contractEvent.ContractVersion);
                    if (approveContract is null)
                    {
                        _logger.LogWarning($"[{nameof(ContractEventProcessor)}] - Ignoring contract event with id [{contractEvent.BookmarkId}] because unable to find a contract with contract number [{contractEvent.ContractNumber}], version [{contractEvent.ContractVersion}].");
                    }
                    else
                    {
                        await _contractApprovalService.ApproveAsync(contractEvent, approveContract);
                    }

                    break;

                case ContractEventType.Withdraw:
                    await _contractWithdrawService.WithdrawAsync(contractEvent);

                    break;

                default:
                    throw new NotImplementedException($"[{nameof(ContractService)}] - [{nameof(ProcessMessage)}] does not have an implementation for event type [{eventType}].");
            }
        }
    }
}