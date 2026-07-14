using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Pds.Contracts.ContractEventProcessor.Services.Models
{
    /// <summary>
    /// Defines the amendment type of the contract.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ContractAmendmentType
    {
        /// <summary>
        /// Defines that this is version 1 of a Contract and has not been changed.
        /// </summary>
        [Display(Name = "")]
        None = 0,

        /// <summary>
        /// Represents a contract change where the contract was approved automatically.
        /// </summary>
        [Display(Name = "Notification")]
        Notification = 1,

        /// <summary>
        /// Represents a contract change where the contract was changed but still needs approval from the provider it belongs too.
        /// </summary>
        [Display(Name = "Variation")]
        Variation = 2
    }
}