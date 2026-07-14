using System.Collections.Generic;

namespace Pds.Contracts.ContractEventProcessor.Services.Models
{
    /// <summary>
    /// Session state.
    /// </summary>
    public class SessionWorkflowState
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is faulted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is faulted; otherwise, <c>false</c>.
        /// </value>
        public bool IsFaulted { get; set; }

        /// <summary>
        /// Gets or sets the failed message identifier.
        /// </summary>
        /// <value>
        /// The failed message identifier.
        /// </value>
        public string FailedMessageId { get; set; }

        /// <summary>
        /// Gets or sets the processed messages.
        /// </summary>
        /// <value>
        /// The processed messages.
        /// </value>
        public string FailedMessageReason { get; set; }

        /// <summary>
        /// Gets or sets the postponed messages.
        /// </summary>
        /// <value>
        /// The postponed messages.
        /// </value>
        public List<string> PostponedMessages { get; set; } = new List<string>();
    }
}