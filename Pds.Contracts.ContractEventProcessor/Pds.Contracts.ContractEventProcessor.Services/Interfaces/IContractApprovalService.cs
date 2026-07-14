using Pds.Contracts.ContractEventProcessor.Services.Models;
using Pds.Contracts.Data.Api.Client.Models;
using System.Threading.Tasks;

namespace Pds.Contracts.ContractEventProcessor.Services.Interfaces
{
    /// <summary>
    /// Contract Approval Service.
    /// </summary>
    public interface IContractApprovalService
    {
        /// <summary>
        /// It will approve the contrack by calling the contract data api.
        /// </summary>
        /// <param name="contractEvent">ContractEvent.</param>
        /// <param name="existingContract">Existing contract.</param>
        /// <returns>Returns True if the contract approved else false.</returns>
        public Task<bool> ApproveAsync(ContractEvent contractEvent, Contract existingContract);
    }
}