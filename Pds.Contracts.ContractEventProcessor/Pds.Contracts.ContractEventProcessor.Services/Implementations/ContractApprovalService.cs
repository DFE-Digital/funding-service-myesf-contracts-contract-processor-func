using Pds.Contracts.ContractEventProcessor.Services.Interfaces;
using Pds.Contracts.ContractEventProcessor.Services.Models;
using Pds.Contracts.Data.Api.Client.Interfaces;
using Pds.Contracts.Data.Api.Client.Models;
using System;
using System.Threading.Tasks;

namespace Pds.Contracts.ContractEventProcessor.Services.Implementations
{
    /// <inheritdoc/>
    public class ContractApprovalService : IContractApprovalService
    {
        private readonly IContractEventProcessorLogger<IContractApprovalService> _logger;

        private readonly IContractsDataService _contractsDataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractApprovalService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="contractsDataService">The contracts data service.</param>
        /// <param name="validationService">The validation service.</param>
        public ContractApprovalService(IContractEventProcessorLogger<IContractApprovalService> logger, IContractsDataService contractsDataService)
        {
            _logger = logger;
            _contractsDataService = contractsDataService;
        }

        /// <inheritdoc/>
        public async Task<bool> ApproveAsync(ContractEvent contractEvent, Contract existingContract)
        {
            var approvalRequest = new ApprovalRequest()
            {
                ContractNumber = existingContract.ContractNumber,
                ContractVersion = existingContract.ContractVersion,
                Id = existingContract.Id,
                FileName = contractEvent.ContractEventXml
            };

            var eventType = contractEvent.GetContractEventType();
            if (eventType != Enums.ContractEventType.Approve)
            {
                throw new InvalidOperationException($"[{nameof(ContractApprovalService)}] - [{nameof(ApproveAsync)}] called for event type [{eventType}].");
            }

            switch (existingContract.Status)
            {
                case Data.Api.Client.Enumerations.ContractStatus.PublishedToProvider:
                    await _contractsDataService.ManualApproveAsync(approvalRequest);
                    break;

                case Data.Api.Client.Enumerations.ContractStatus.ApprovedWaitingConfirmation:
                    await _contractsDataService.ConfirmApprovalAsync(approvalRequest);
                    break;

                default:
                    _logger.LogInformation($"[{nameof(ContractApprovalService)}] - [{nameof(ApproveAsync)}] - No further action taken on [{existingContract.ContractNumber}], version [{existingContract.ContractVersion}], Id [{existingContract.Id}], event parent status [{contractEvent.ParentStatus}], event status [{contractEvent.Status}], event amendment type [{contractEvent.AmendmentType}] and contract status [{existingContract.Status}].");
                    break;
            }

            return true;
        }
    }
}