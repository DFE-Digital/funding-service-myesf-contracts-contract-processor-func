namespace Pds.Contracts.ContractEventProcessor.Services.Models
{
    /// <summary>
    /// Represents Contract Allocation data.
    /// </summary>
    public class ContractAllocation
    {
        /// <summary>
        /// Gets or sets the Contract Allocation Number.
        /// </summary>
        public string ContractAllocationNumber { get; set; }

        /// <summary>
        /// Gets or sets funding Stream Preriod Code.
        /// </summary>
        public string FundingStreamPeriodCode { get; set; }

        /// <summary>
        /// Gets or sets LEP Area.
        /// </summary>
        public string LEPArea { get; set; }

        /// <summary>
        /// Gets or sets Tender Spec Title.
        /// </summary>
        public string TenderSpecTitle { get; set; }
    }
}