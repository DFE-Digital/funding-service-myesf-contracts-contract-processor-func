namespace Pds.Contracts.ContractEventProcessor.Services.Interfaces
{
    /// <summary>
    /// Function settings interface.
    /// </summary>
    public interface IFunctionSettings
    {
        /// <summary>
        /// Gets or sets the maximum delivery count.
        /// </summary>
        int MaximumDeliveryCount { get; set; }
    }
}