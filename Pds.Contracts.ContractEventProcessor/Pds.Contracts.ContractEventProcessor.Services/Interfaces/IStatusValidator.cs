using Pds.Contracts.ContractEventProcessor.Services.Models;

namespace Pds.Contracts.ContractEventProcessor.Services.Interfaces
{
    /// <summary>
    /// The contract event status validator.
    /// </summary>
    public interface IStatusValidator
    {
        /// <summary>
        /// Validate the contract event for the Parent status, Status and the Amendment type.
        /// </summary>
        /// <param name="contractEvent">The ContractEvent data from the service bus.</param>
        /// <returns>Returns true if the criteria meet else false. </returns>
        public bool Validate(ContractEvent contractEvent);
    }
}