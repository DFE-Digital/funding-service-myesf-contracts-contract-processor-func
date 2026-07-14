using Pds.Contracts.ContractEventProcessor.Services.Interfaces;

namespace Pds.Contracts.ContractEventProcessor.Services.Models
{
    /// <summary>
    /// Configuation settings.
    /// </summary>
    public class FunctionSettings : IFunctionSettings
    {
        /// <inheritdoc/>
        public int MaximumDeliveryCount { get; set; }
    }
}