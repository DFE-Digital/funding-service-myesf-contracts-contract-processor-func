using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Pds.Contracts.ContractEventProcessor.Services.CustomExceptionHandlers
{
    /// <summary>
    /// Api client token acquisition failure exception.
    /// <seealso cref="Exception"/>.
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class SPTokenAcquisitionFailureException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SPTokenAcquisitionFailureException"/> class.
        /// </summary>
        public SPTokenAcquisitionFailureException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SPTokenAcquisitionFailureException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public SPTokenAcquisitionFailureException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SPTokenAcquisitionFailureException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public SPTokenAcquisitionFailureException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SPTokenAcquisitionFailureException"/> class.
        /// </summary>
        /// <param name="info">info.</param>
        /// <param name="context">context.</param>
        protected SPTokenAcquisitionFailureException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}