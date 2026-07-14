using System.Threading.Tasks;

namespace Pds.Contracts.ContractEventProcessor.Services.Interfaces
{
    /// <summary>
    /// It will read a list and download a file from the sharepoint.
    /// </summary>
    public interface ISharePointClientService
    {
        /// <summary>
        /// Check and download file form the sharepoint.
        /// </summary>
        /// <param name="filename">Share point file name.</param>
        /// <param name="libraryName">Share Point list name.</param>
        /// <returns>pdf contract file.</returns>
        Task<byte[]> GetDocument(string filename, string libraryName);
    }
}