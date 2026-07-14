using Microsoft.Extensions.Logging;
using Pds.Contracts.ContractEventProcessor.Services.Interfaces;
using System;

namespace Pds.Contracts.ContractEventProcessor.Services.Implementations
{
    /// <summary>
    /// Custom logger to enrich log messages with contract event processing metadata.
    /// </summary>
    /// <typeparam name="T">Logger category.</typeparam>
    /// <seealso cref="Pds.Contracts.ContractEventProcessor.Services.Interfaces.IContractEventProcessorLogger{T}" />
    public class ContractEventProcessorLogger<T> : IContractEventProcessorLogger<T>
    {
        private readonly IContractEventProcessLog _processLog;
        private readonly ILogger<T> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractEventProcessorLogger{T}"/> class.
        /// </summary>
        /// <param name="processLog">The process log.</param>
        /// <param name="logger">The logger.</param>
        public ContractEventProcessorLogger(IContractEventProcessLog processLog, ILogger<T> logger)
        {
            _processLog = processLog;
            _logger = logger;
        }

        /// <inheritdoc/>
        public void LogError(Exception ex, string message)
        {
            _logger.LogError(ex, _processLog.CreateLogMessage(message));
        }

        /// <inheritdoc/>
        public void LogError(string message)
        {
            _logger.LogError(_processLog.CreateLogMessage(message));
        }

        /// <inheritdoc/>
        public void LogInformation(string message)
        {
            _logger.LogInformation(_processLog.CreateLogMessage(message));
        }

        /// <inheritdoc/>
        public void LogWarning(string message)
        {
            _logger.LogWarning(_processLog.CreateLogMessage(message));
        }

        /// <inheritdoc/>
        public void LogWarning(Exception ex, string message)
        {
            _logger.LogWarning(ex, _processLog.CreateLogMessage(message));
        }
    }
}