using System;
using System.Diagnostics.CodeAnalysis;

namespace Pds.Contracts.ContractEventProcessor.Services.CustomExceptionHandlers
{
    /// <summary>
    /// Document not found exception.
    /// <seealso cref="Exception"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DocumentNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public DocumentNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public DocumentNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}