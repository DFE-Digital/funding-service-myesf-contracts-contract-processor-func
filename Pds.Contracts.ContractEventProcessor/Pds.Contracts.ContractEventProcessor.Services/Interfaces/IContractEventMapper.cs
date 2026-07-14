using Pds.Contracts.ContractEventProcessor.Services.Models;
using Pds.Contracts.Data.Api.Client.Models;
using System;
using System.Collections.Generic;

namespace Pds.Contracts.ContractEventProcessor.Services.Interfaces
{
    /// <summary>
    /// The Contract Processor Service.
    /// </summary>
    public interface IContractEventMapper
    {
        /// <summary>
        /// Create Contract Title.
        /// </summary>
        /// <param name="contractEvent">ContractEvent.</param>
        /// <returns>Returns contract title.</returns>
        string CreateContractTitle(ContractEvent contractEvent);

        /// <summary>
        /// Change Period from 1516 to "2015 to 2016".
        /// </summary>
        /// <param name="contractEvent">The contract event.</param>
        /// <returns>
        /// Returns a formatted period.
        /// </returns>
        string FormatPeriod(ContractEvent contractEvent);

        /// <summary>
        /// GetContractContent - Get the contract pdf file from the Sharepoint.
        /// </summary>
        /// <param name="pdfADoc">The contract pdfA document file.</param>
        /// <param name="fileName">The contract pdf document file name.</param>
        /// <returns>Returns CreateContractRequestDocument object.</returns>
        CreateContractRequestDocument GetContractContent(byte[] pdfADoc, string fileName);

        /// <summary>
        /// Get array of unique Contract Funding Stream Period Codes.
        /// </summary>
        /// <param name="contractAllocations">IEnumerable of ContractAllocations.</param>
        /// <returns>Returns array of CreateContractCode.</returns>
        CreateContractCode[] GetContractFundingStreamPeriodCodes(IEnumerable<ContractAllocation> contractAllocations);

        /// <summary>
        /// Get CreateRequest objects from the ContractEvent.
        /// </summary>
        /// <param name="contractEvent">ContractEvent.</param>
        /// <returns>Returns CreateRequest.</returns>
        CreateRequest GetCreateRequest(ContractEvent contractEvent);

        /// <summary>
        /// Get file name for the contract document.
        /// </summary>
        /// <param name="ukprn">The ukprn.</param>
        /// <param name="contractNumber">The contract number.</param>
        /// <param name="version">The contract version.</param>
        /// <returns>Returns the pdf file name.</returns>
        string GetFileNameForContractDocument(int? ukprn, string contractNumber, int version);

        /// <summary>
        /// Get folder name for the contract document Which is a sharepoint document title.
        /// </summary>
        /// <param name="fundingTypeShortName">The contract funding type short name attributes.</param>
        /// <param name="period">The period.</param>
        /// <param name="suffix">The suffix text to be added at the end.</param>
        /// <returns>Returns sharepoint document title text.</returns>
        string GetFolderNameForContractDocument(string fundingTypeShortName, string period, string suffix);

        /// <summary>
        /// Get user friendly Year.
        /// </summary>
        /// <param name="year">From and To year.</param>
        /// <returns>Return user readable year.</returns>
        string GetFriendlyYear(string year);

        /// <summary>
        /// Get starting Year.
        /// </summary>
        /// <param name="periodValue">The period value.</param>
        /// <returns>Returns Year.</returns>
        int GetStartingYear(string periodValue);
    }
}