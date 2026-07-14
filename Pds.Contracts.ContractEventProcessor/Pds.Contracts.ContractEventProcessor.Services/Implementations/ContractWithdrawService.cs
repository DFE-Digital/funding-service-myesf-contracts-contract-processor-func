using Pds.Contracts.ContractEventProcessor.Services.CustomExceptionHandlers;
using Pds.Contracts.ContractEventProcessor.Services.Interfaces;
using Pds.Contracts.ContractEventProcessor.Services.Models;
using Pds.Contracts.Data.Api.Client.Exceptions;
using Pds.Contracts.Data.Api.Client.Interfaces;
using Pds.Contracts.Data.Api.Client.Models;
using System.Threading.Tasks;
using ClientEnums = Pds.Contracts.Data.Api.Client.Enumerations;

namespace Pds.Contracts.ContractEventProcessor.Services.Implementations
{
    /// <inheritdoc/>
    public class ContractWithdrawService : IContractWithdrawService
    {
        private readonly IContractEventProcessorLogger<IContractWithdrawService> _logger;

        private readonly IContractsDataService _contractsDataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractWithdrawService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="contractsDataService">The contracts data service.</param>
        /// <param name="validationService">The validation service.</param>
        public ContractWithdrawService(IContractEventProcessorLogger<IContractWithdrawService> logger, IContractsDataService contractsDataService)
        {
            _logger = logger;
            _contractsDataService = contractsDataService;
        }

        /// <inheritdoc/>
        public async Task<bool> WithdrawAsync(ContractEvent contractEvent)
        {
            try
            {
                var withdrawRequest = new WithdrawalRequest()
                {
                    ContractNumber = contractEvent.ContractNumber,
                    ContractVersion = contractEvent.ContractVersion,
                    FileName = contractEvent.ContractEventXml,
                    WithdrawalType = (ClientEnums.ContractStatus)contractEvent.Status
                };

                await _contractsDataService.WithdrawAsync(withdrawRequest);

                return true;
            }
            catch (ContractNotFoundClientException ex)
            {
                throw new ContractEventExpectationFailedException(contractEvent.BookmarkId, contractEvent.ContractNumber, contractEvent.ContractVersion, $"Withdraw called when a contract cannot be found.", ex);
            }
        }
    }
}