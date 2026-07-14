using Pds.Contracts.ContractEventProcessor.Services.Enums;
using Pds.Contracts.ContractEventProcessor.Services.Models;
using System;

namespace Pds.Contracts.ContractEventProcessor.Services.Implementations
{
    /// <summary>
    /// Extension method for contract event.
    /// </summary>
    public static class ContractEventExtension
    {
        /// <summary>
        /// Gets the type of the contract event.
        /// </summary>
        /// <param name="contractEvent">The contract event.</param>
        /// <returns><see cref="ContractEventType"/> value based on parent status, status and ammendment type.</returns>
        public static ContractEventType GetContractEventType(this ContractEvent contractEvent)
        {
            var eventType = (contractEvent.ParentStatus, contractEvent.Status, contractEvent.AmendmentType) switch
            {
                (ContractParentStatus.Draft, ContractStatus.PublishedToProvider, ContractAmendmentType.None) => ContractEventType.Create,
                (ContractParentStatus.Draft, ContractStatus.PublishedToProvider, ContractAmendmentType.Variation) => ContractEventType.Create,
                (ContractParentStatus.Approved, ContractStatus.Approved, ContractAmendmentType.Notification) => ContractEventType.Create,
                (ContractParentStatus.Approved, ContractStatus.Modified, ContractAmendmentType.Notification) => ContractEventType.Create,
                (ContractParentStatus.Approved, ContractStatus.UnderTermination, ContractAmendmentType.Notification) => ContractEventType.Create,

                (ContractParentStatus.Approved, ContractStatus.Approved, ContractAmendmentType.None) => ContractEventType.Approve,
                (ContractParentStatus.Approved, ContractStatus.Approved, ContractAmendmentType.Variation) => ContractEventType.Approve,
                (ContractParentStatus.Approved, ContractStatus.Modified, ContractAmendmentType.None) => ContractEventType.Approve,
                (ContractParentStatus.Approved, ContractStatus.Modified, ContractAmendmentType.Variation) => ContractEventType.Approve,
                (ContractParentStatus.Approved, ContractStatus.UnderTermination, ContractAmendmentType.None) => ContractEventType.Approve,
                (ContractParentStatus.Approved, ContractStatus.UnderTermination, ContractAmendmentType.Variation) => ContractEventType.Approve,

                (ContractParentStatus.Withdrawn, ContractStatus.WithdrawnByAgency, ContractAmendmentType.None) => ContractEventType.Withdraw,
                (ContractParentStatus.Withdrawn, ContractStatus.WithdrawnByAgency, ContractAmendmentType.Variation) => ContractEventType.Withdraw,
                (ContractParentStatus.Withdrawn, ContractStatus.WithdrawnByProvider, ContractAmendmentType.None) => ContractEventType.Withdraw,
                (ContractParentStatus.Withdrawn, ContractStatus.WithdrawnByProvider, ContractAmendmentType.Variation) => ContractEventType.Withdraw,

                _ => throw new NotImplementedException($"BookmarkId: [{contractEvent.BookmarkId}] with contract number {contractEvent.ContractNumber} and version [{contractEvent.ContractVersion}] contains unexpected combination of ParentStatus: {contractEvent.ParentStatus}, Status: {contractEvent.Status} and AmendmentType: {contractEvent.AmendmentType}. This combination does not have a corresponding imeplementation defiend."),
            };
            return eventType;
        }
    }
}