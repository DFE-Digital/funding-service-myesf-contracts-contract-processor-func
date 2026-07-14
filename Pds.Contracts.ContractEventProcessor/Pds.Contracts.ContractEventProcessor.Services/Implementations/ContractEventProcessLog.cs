using Microsoft.Azure.ServiceBus;
using Pds.Contracts.ContractEventProcessor.Services.Configurations;
using Pds.Contracts.ContractEventProcessor.Services.Interfaces;
using Pds.Contracts.ContractEventProcessor.Services.Models;

namespace Pds.Contracts.ContractEventProcessor.Services.Implementations
{
    /// <summary>
    /// Contract event processing log, to prefix with message id, bookmark id and session details for easier tracing.
    /// </summary>
    /// <seealso cref="Pds.Contracts.ContractEventProcessor.Services.Interfaces.IContractEventProcessLog" />
    public class ContractEventProcessLog : IContractEventProcessLog
    {
        private Message _message;
        private ContractEvent _contractEvent;
        private bool _hasInitialised = false;

        /// <inheritdoc/>
        public string CreateLogMessage(string log)
        {
            return _hasInitialised
                ? $"{log} - Metadata- RunId: [{_message?.UserProperties[Constants.ContractEventProcessorRunIdKey]}] - Attempt:[{_message?.SystemProperties.DeliveryCount}] for message: [{_message?.MessageId}] in session [{_message?.SessionId}] with contract event bookmark [{_contractEvent?.BookmarkId}]"
                : log;
        }

        /// <inheritdoc/>
        public void Initialise(Message message, ContractEvent contractEvent)
        {
            _message = message;
            _contractEvent = contractEvent;
            _hasInitialised = true;
        }
    }
}