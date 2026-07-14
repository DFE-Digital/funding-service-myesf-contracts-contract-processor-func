using Pds.Contracts.ContractEventProcessor.Services.Models;
using System.Threading.Tasks;

namespace Pds.Contracts.ContractEventProcessor.Services.Interfaces
{
    /// <summary>
    /// Contract Creation Service.
    /// </summary>
    public interface IContractCreationService
    {
        /// <summary>
        /// It will call the contract data API to create new contract.
        /// </summary>
        /// <param name="contractEvent">contractEvent.</param>
        /// <returns>Returns true if the contract created, else false.</returns>
        public Task<bool> CreateAsync(ContractEvent contractEvent);
    }
}