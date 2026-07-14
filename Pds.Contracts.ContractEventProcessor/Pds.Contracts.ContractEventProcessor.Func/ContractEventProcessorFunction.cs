using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Pds.Contracts.ContractEventProcessor.Services.Configurations;
using Pds.Contracts.ContractEventProcessor.Services.Interfaces;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Pds.Contracts.ContractEventProcessor.Func
{
    /// <summary>
    /// Example ServiceBus queue triggered Azure Function.
    /// </summary>
    public class ContractEventProcessorFunction
    {
        private readonly IContractEventSessionManager _contractEventSessionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractEventProcessorFunction" /> class.
        /// </summary>
        /// <param name="contractEventSessionManager">The contract event session manager.</param>
        public ContractEventProcessorFunction(IContractEventSessionManager contractEventSessionManager)
        {
            _contractEventSessionManager = contractEventSessionManager;
        }

        /// <summary>
        /// Entry point to the Azure Function.
        /// </summary>
        /// <param name="contractEvent">The queue item that triggered this function to run.</param>
        /// <param name="messageSession">The message session.</param>
        /// <param name="log">The logger.</param>
        /// <returns>
        /// Async Task.
        /// </returns>
        /// <remarks>
        /// The <see cref="ExponentialBackoffRetryAttribute"/> allows the function to be invoked if the message fails to process.
        /// The number of retries specified here is multiplative with the message bus max delivery count.
        /// When a message fails to process, the function will attempt to retry the number of attempts specified here.
        /// The message will then be returned to the queue and it's delivery count incremented by 1.
        /// As such, setting the retry value to 5, and a max delivery count of 10 on the service bus will result in a total of 50 attempts to process the message.
        /// This retry count applies to all messages, including malformed or bad messages.
        /// see - https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-error-pages?tabs=csharp#using-retry-support-on-top-of-trigger-resilience.
        /// </remarks>
        [FunctionName(nameof(ContractEventProcessorFunction))]
        [ExponentialBackoffRetry(5, "00:00:05", "00:15:00")]
        public async Task Run(
            [ServiceBusTrigger("%ContractEventsSessionQueue%", Connection = "ServiceBusConnection", IsSessionsEnabled = true)] Message contractEvent,
            IMessageSession messageSession,
            ILogger log)
        {
            var watch = Stopwatch.StartNew();

            if (!contractEvent.UserProperties.TryGetValue(Constants.ContractEventProcessorRunIdKey, out var runId))
            {
                runId ??= Guid.NewGuid();
                contractEvent.UserProperties.Add(Constants.ContractEventProcessorRunIdKey, runId);
            }

            using (log?.BeginScope($"[{nameof(ContractEventProcessorFunction)}] - RunId: [{runId}], message: [{contractEvent.MessageId}],  and session: [{messageSession.SessionId}] attempt: [{contractEvent.SystemProperties.DeliveryCount}]"))
            {
                log?.LogInformation($"Start: Processing message: {contractEvent.MessageId} from session: {messageSession.SessionId}.");

                var state = await _contractEventSessionManager.ProcessSessionMessageAsync(messageSession, contractEvent);

                watch.Stop();
                if (state.IsFaulted)
                {
                    log?.LogWarning($"[{nameof(ContractEventProcessorFunction)}] - End (spent: {watch.Elapsed}): Message: {contractEvent.MessageId} moved to DLQ from session: {messageSession.SessionId}.");
                }
                else
                {
                    log?.LogInformation($"[{nameof(ContractEventProcessorFunction)}] - End (spent: {watch.Elapsed}): Successfully processed message: {contractEvent.MessageId} from session: {messageSession.SessionId}.");
                }
            }
        }
    }
}