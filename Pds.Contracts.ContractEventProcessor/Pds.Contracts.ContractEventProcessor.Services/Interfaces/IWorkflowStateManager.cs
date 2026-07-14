using Microsoft.Azure.ServiceBus;
using Pds.Contracts.ContractEventProcessor.Services.Models;
using System.Threading.Tasks;

namespace Pds.Contracts.ContractEventProcessor.Services.Interfaces
{
    /// <summary>
    /// Interface contract for workflow state manager.
    /// </summary>
    public interface IWorkflowStateManager
    {
        /// <summary>
        /// Gets the workflow state asynchronously.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns>Workflow state <see cref="SessionWorkflowState"/> from session <see cref="IMessageSession"/>.</returns>
        Task<SessionWorkflowState> GetWorkflowStateAsync(IMessageSession session);

        /// <summary>
        /// Sets the workflow state asynchronous.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="state">The state.</param>
        /// <returns>Async task.</returns>
        Task SetWorkflowStateAsync(IMessageSession session, SessionWorkflowState state);

        /// <summary>
        /// Resets the workflow state asynchronously.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns>Async task.</returns>
        Task<SessionWorkflowState> ResetWorkflowStateAsync(IMessageSession session);
    }
}