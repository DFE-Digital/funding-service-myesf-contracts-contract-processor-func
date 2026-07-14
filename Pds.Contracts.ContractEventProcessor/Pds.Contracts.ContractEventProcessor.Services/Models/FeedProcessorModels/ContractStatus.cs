using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Pds.Contracts.ContractEventProcessor.Services.Models
{
    /// <summary>
    /// Represents the states a contract can be in.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ContractStatus
    {
        /// <summary>
        /// The contract is now ready to be signed by the Provider.
        /// </summary>
        [Display(Name = "Ready to sign", Description = "Contract ready to be signed")]
        PublishedToProvider = 0,

        /// <summary>
        /// The contract has been withdrawn by CDS.
        /// </summary>
        [Display(Name = "withdrawn by agency", Description = "Contract withdrawn CDS requested")]
        WithdrawnByAgency = 1,

        /// <summary>
        /// The contract has been withdrawn by Provider.
        /// </summary>
        [Display(Name = "withdrawn by provider", Description = "Contract withdrawn provider requested")]
        WithdrawnByProvider = 2,

        /// <summary>
        /// The contract has been withdrawn through schedule.
        /// </summary>
        [Display(Name = "Autowithdrawn", Description = "Contract withdrawn missed deadline")]
        [Obsolete("This status has been made obsolete, do not use, instead use WithdrawnByAgency or WithdrawnByProvider.")]
        AutoWithdrawn = 3,

        /// <summary>
        /// The contract is now approved.
        /// </summary>
        [Display(Name = "Approved", Description = "Contract approved")]
        Approved = 4,

        /// <summary>
        /// The contract has been signed by provider and is awaiting FCS approval.
        /// </summary>
        [Display(Name = "Approved", Description = "Contract approved")]
        ApprovedWaitingConfirmation = 5,

        /// <summary>
        /// The contract has been replaced with a notification or variation.
        /// </summary>
        [Display(Name = "Replaced", Description = "Contract replaced")]
        Replaced = 6,

        /// <summary>
        /// The contract has been modified.
        /// </summary>
        [Display(Name = "Modified", Description = "Modified")]
        Modified = 7,

        /// <summary>
        /// The contract is under termination
        /// </summary>
        [Display(Name = "Under Termination", Description = "Under Termination")]
        UnderTermination = 8,

        /// <summary>
        /// Contract closed.
        /// </summary>
        [Display(Name = "Closed", Description = "Closed")]
        Closed,

        /// <summary>
        /// Contract terminated.
        /// </summary>
        [Display(Name = "Terminated", Description = "Terminated")]
        Terminated,

        /// <summary>
        /// Contract is draft.
        /// </summary>
        Draft,

        /// <summary>
        /// Contract is unassigned.
        /// </summary>
        Unassigned,

        /// <summary>
        /// Contract is in review.
        /// </summary>
        InReview,

        /// <summary>
        /// Contract is awaiting internal approval
        /// </summary>
        AwaitingInternalApproval,
    }
}