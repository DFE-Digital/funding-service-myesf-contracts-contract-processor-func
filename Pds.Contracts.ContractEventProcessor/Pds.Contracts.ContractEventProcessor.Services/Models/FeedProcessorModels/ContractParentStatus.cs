using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace Pds.Contracts.ContractEventProcessor.Services.Models
{
    /// <summary>
    /// Represents the states for a contract parent.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ContractParentStatus
    {
        /// <summary>
        /// Contract is in draft state.
        /// </summary>
        Draft,

        /// <summary>
        /// Contract has been approved.
        /// </summary>
        Approved,

        /// <summary>
        /// Contract has been withdrawn.
        /// </summary>
        Withdrawn,

        /// <summary>
        /// Contract has been closed.
        /// </summary>
        Closed
    }
}