using System;
using System.IO;

namespace Pds.Contracts.ContractEventProcessor.Services.Extensions
{
    /// <summary>
    /// The Stream Extensions.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Stream to byte array.
        /// </summary>
        /// <param name="stream">The input stream.</param>
        /// <returns>Returns byte array.</returns>
        public static byte[] ToByteArray(this Stream stream)
        {
            stream.Position = 0;
            byte[] buffer = new byte[stream.Length];
            for (int totalBytesCopied = 0; totalBytesCopied < stream.Length;)
            {
                totalBytesCopied += stream.Read(buffer, totalBytesCopied, Convert.ToInt32(stream.Length) - totalBytesCopied);
            }

            return buffer;
        }
    }
}