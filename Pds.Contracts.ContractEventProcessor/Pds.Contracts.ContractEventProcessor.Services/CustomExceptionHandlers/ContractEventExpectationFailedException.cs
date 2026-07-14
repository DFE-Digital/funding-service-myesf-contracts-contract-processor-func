using System;
using System.Runtime.Serialization;

namespace Pds.Contracts.ContractEventProcessor.Services.CustomExceptionHandlers
{
    /// <summary>
    /// Contract event expectation failed exception.
    /// </summary>
    [Serializable]
    public class ContractEventExpectationFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContractEventExpectationFailedException" /> class.
        /// </summary>
        /// <param name="bookmarkId">The bookmark identifier.</param>
        /// <param name="contractNumber">Contract number.</param>
        /// <param name="versionNumber">Contract version number.</param>
        /// <param name="failedExpectation">Failed validation expression.</param>
        public ContractEventExpectationFailedException(Guid bookmarkId, string contractNumber, int versionNumber, string failedExpectation)
            : base($"Expectation: {failedExpectation} failed in contract event [{bookmarkId}] with contract number:{contractNumber}, version: {versionNumber}.")
        {
            ContractNumber = contractNumber;
            VersionNumber = versionNumber;
            FailedExpectation = failedExpectation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractEventExpectationFailedException"/> class.
        /// </summary>
        /// <param name="bookmarkId">The bookmark identifier.</param>
        /// <param name="contractNumber">The contract number.</param>
        /// <param name="versionNumber">The version number.</param>
        /// <param name="failedExpectation">The failed expectation.</param>
        /// <param name="innerException">The inner exception.</param>
        public ContractEventExpectationFailedException(Guid bookmarkId, string contractNumber, int versionNumber, string failedExpectation, Exception innerException)
            : base($"Expectation: {failedExpectation} failed in contract event [{bookmarkId}] with contract number:{contractNumber}, version: {versionNumber}.", innerException)
        {
            ContractNumber = contractNumber;
            VersionNumber = versionNumber;
            FailedExpectation = failedExpectation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractEventExpectationFailedException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected ContractEventExpectationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Gets ContractNumber.
        /// </summary>
        public string ContractNumber { get; }

        /// <summary>
        /// Gets VersionNumber.
        /// </summary>
        public int VersionNumber { get; }

        /// <summary>
        /// Gets Predicate.
        /// </summary>
        public string FailedExpectation { get; }
    }
}