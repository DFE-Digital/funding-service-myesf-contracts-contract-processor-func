using Microsoft.Azure.ServiceBus;
using Pds.Contracts.ContractEventProcessor.Services.Models;
using System.Threading.Tasks;

namespace Pds.Contracts.ContractEventProcessor.Services.Interfaces
{
    /// <summary>
    /// Manages message session for contract events.
    /// </summary>
    public interface IContractEventSessionManager
    {
        /// <summary>
        /// Processes the session message asynchronously.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="message">The message.</param>
        /// <returns>Async task.</returns>
        Task<SessionWorkflowState> ProcessSessionMessageAsync(IMessageSession session, Message message);
    }
}