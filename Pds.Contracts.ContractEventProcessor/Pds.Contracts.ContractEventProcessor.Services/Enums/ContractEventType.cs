namespace Pds.Contracts.ContractEventProcessor.Services.Enums
{
    /// <summary>
    /// Contract Event Type.
    /// </summary>
    public enum ContractEventType
    {
        /// <summary>
        /// Contract event type has not yet been defined.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Contract creation event.
        /// </summary>
        Create,

        /// <summary>
        /// Contract approval event.
        /// </summary>
        Approve,

        /// <summary>
        /// Contract withdraw event.
        /// </summary>
        Withdraw
    }
}