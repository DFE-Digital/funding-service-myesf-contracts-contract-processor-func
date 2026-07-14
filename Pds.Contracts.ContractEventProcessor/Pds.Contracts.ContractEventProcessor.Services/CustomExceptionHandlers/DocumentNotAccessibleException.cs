using System;
using System.Diagnostics.CodeAnalysis;

namespace Pds.Contracts.ContractEventProcessor.Services.CustomExceptionHandlers
{
    /// <summary>
    /// Document not accessible exception.
    /// <seealso cref="Exception"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DocumentNotAccessibleException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentNotAccessibleException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public DocumentNotAccessibleException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentNotAccessibleException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public DocumentNotAccessibleException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}