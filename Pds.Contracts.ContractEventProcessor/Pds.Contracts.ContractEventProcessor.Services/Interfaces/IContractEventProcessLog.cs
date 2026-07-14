using Microsoft.Azure.ServiceBus;
using Pds.Contracts.ContractEventProcessor.Services.Models;

namespace Pds.Contracts.ContractEventProcessor.Services.Interfaces
{
    /// <summary>
    /// Interface to log messages during ContractEvent processing.
    /// </summary>
    public interface IContractEventProcessLog
    {
        /// <summary>
        /// Initialises the specified log.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="contractEvent">The contract event.</param>
        void Initialise(Message message, ContractEvent contractEvent);

        /// <summary>
        /// Creates the log message with contract event specific information to support tracing.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <returns>Formatted log message.</returns>
        string CreateLogMessage(string log);
    }
}