using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pds.Contracts.ContractEventProcessor.Services.CustomExceptionHandlers;
using Pds.Contracts.ContractEventProcessor.Services.Interfaces;
using Pds.Contracts.ContractEventProcessor.Services.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Pds.Contracts.ContractEventProcessor.Services.Implementations
{
    /// <summary>
    /// Contract events session message manager.
    /// </summary>
    /// <seealso cref="Pds.Contracts.ContractEventProcessor.Services.Interfaces.IContractEventSessionManager" />
    public class ContractEventSessionManager : IContractEventSessionManager
    {
        private readonly IWorkflowStateManager _stateManager;
        private readonly IContractService _contractService;
        private readonly IContractEventProcessorLogger<IContractEventSessionManager> _logger;
        private readonly IFunctionSettings _functionSettings;
        private readonly IContractEventProcessLog _processLog;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractEventSessionManager" /> class.
        /// </summary>
        /// <param name="stateManager">The session workflow state manager.</param>
        /// <param name="contractService">The contract service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="functionSettings">The configuration settings options.</param>
        /// <param name="processLog">The process log.</param>
        public ContractEventSessionManager(
            IWorkflowStateManager stateManager,
            IContractService contractService,
            IContractEventProcessorLogger<IContractEventSessionManager> logger,
            IFunctionSettings functionSettings,
            IContractEventProcessLog processLog)
        {
            _stateManager = stateManager;
            _contractService = contractService;
            _logger = logger;
            _functionSettings = functionSettings;
            _processLog = processLog;
        }

        /// <inheritdoc/>
        public async Task<SessionWorkflowState> ProcessSessionMessageAsync(IMessageSession session, Message message)
        {
            var state = await _stateManager.GetWorkflowStateAsync(session);
            if (message.UserProperties.TryGetValue("ResetWorkflowState", out var shouldReset) && Convert.ToBoolean(shouldReset))
            {
                _logger.LogInformation($"[{nameof(ContractEventSessionManager)}.{nameof(ProcessSessionMessageAsync)}] - Reset workflow state message [{message.MessageId}] received, resetting session [{session.SessionId}].");
                return await _stateManager.ResetWorkflowStateAsync(session);
            }

            if (state.IsFaulted && state.FailedMessageId != message.MessageId)
            {
                _logger.LogWarning($"[{nameof(ContractEventSessionManager)}.{nameof(ProcessSessionMessageAsync)}] - Moving this message [{message.MessageId}] to DLQ, beacause previous message [{state.FailedMessageId}] failed, hence dropping all messages in session [{session.SessionId}].");
                state.PostponedMessages.Add(message.MessageId);
                await session.DeadLetterAsync(message.SystemProperties.LockToken, $"Previous message failed, hence dropping all messages in session", $"Previous message {state.FailedMessageId} failed, hence dropping all messages in session {session.SessionId}");
                await _stateManager.SetWorkflowStateAsync(session, state);
            }
            else
            {
                if (state.IsFaulted && state.FailedMessageId == message.MessageId)
                {
                    state = await _stateManager.ResetWorkflowStateAsync(session);
                }

                try
                {
                    var contractEvent = JsonConvert.DeserializeObject<ContractEvent>(Encoding.UTF8.GetString(message.Body));
                    _processLog.Initialise(message, contractEvent);

                    await _contractService.ProcessMessage(contractEvent);
                }
                catch (NotImplementedException notImplemented)
                {
                    await SaveFailedState(session, message, state, notImplemented, "Message contains not implemented input");
                    await session.DeadLetterAsync(message.SystemProperties.LockToken, $"Message contains not imeplemented input", $"Invalid message {state.FailedMessageId} in session {session.SessionId} reason - {notImplemented.Message}.");
                }
                catch (ContractEventExpectationFailedException badMessage)
                {
                    await SaveFailedState(session, message, state, badMessage, "Message contains invalid input");
                    await session.DeadLetterAsync(message.SystemProperties.LockToken, $"Message contains invalid input", $"Invalid message {state.FailedMessageId} in session {session.SessionId} reason - {badMessage.Message}.");
                }
                catch (Exception ex)
                {
                    if (message.SystemProperties.DeliveryCount >= _functionSettings.MaximumDeliveryCount)
                    {
                        await SaveFailedState(session, message, state, ex, "Message delivery count exceeded");
                    }

                    throw;
                }
            }

            return state;
        }

        private async Task SaveFailedState(IMessageSession session, Message message, SessionWorkflowState state, Exception ex, string reason)
        {
            state.FailedMessageId = message.MessageId;
            state.IsFaulted = true;
            state.FailedMessageReason = $"{reason} with error [{ex.Message}]";
            await _stateManager.SetWorkflowStateAsync(session, state);

            var logMessage = $"[{nameof(ContractEventSessionManager)}.{nameof(ProcessSessionMessageAsync)}] - {reason} saving workflow state as faulted with failed message : {message.MessageId} for session: {session.SessionId}. With Exception: [{ex}]";
            if (ex is ContractEventExpectationFailedException)
            {
                _logger.LogError(ex, logMessage);
            }
            else
            {
                _logger.LogWarning(ex, logMessage);
            }
        }
    }
}