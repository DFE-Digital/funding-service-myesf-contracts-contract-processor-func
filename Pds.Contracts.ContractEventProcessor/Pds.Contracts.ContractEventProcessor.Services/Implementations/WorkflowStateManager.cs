using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Pds.Contracts.ContractEventProcessor.Services.Interfaces;
using Pds.Contracts.ContractEventProcessor.Services.Models;
using System.Text;
using System.Threading.Tasks;

namespace Pds.Contracts.ContractEventProcessor.Services.Implementations
{
    /// <summary>
    /// Manage workflow state.
    /// </summary>
    /// <seealso cref="Pds.Contracts.ContractEventProcessor.Services.Interfaces.IWorkflowStateManager" />
    public class WorkflowStateManager : IWorkflowStateManager
    {
        /// <inheritdoc/>
        public async Task SetWorkflowStateAsync(IMessageSession session, SessionWorkflowState state)
        {
            byte[] bytes = null;
            if (state != null)
            {
                var json = JsonConvert.SerializeObject(state);
                bytes = Encoding.UTF8.GetBytes(json);
            }

            await session.SetStateAsync(bytes);
        }

        /// <inheritdoc/>
        public async Task<SessionWorkflowState> GetWorkflowStateAsync(IMessageSession session)
        {
            byte[] bytes = await session.GetStateAsync();

            var state = new SessionWorkflowState();
            if (bytes != null)
            {
                var json = Encoding.UTF8.GetString(bytes);
                state = JsonConvert.DeserializeObject<SessionWorkflowState>(json);
            }

            return state;
        }

        /// <inheritdoc/>
        public async Task<SessionWorkflowState> ResetWorkflowStateAsync(IMessageSession session)
        {
            await session.SetStateAsync(null);
            return new SessionWorkflowState();
        }
    }
}