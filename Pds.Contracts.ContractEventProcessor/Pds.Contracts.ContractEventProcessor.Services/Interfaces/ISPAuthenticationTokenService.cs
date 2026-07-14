using System.Threading.Tasks;

namespace Pds.Contracts.ContractEventProcessor.Services.Interfaces
{
    /// <summary>
    /// The SharePoint authentication token serivce.
    /// </summary>
    public interface ISPAuthenticationTokenService
    {
        /// <summary>
        /// Get sharepoint token from the account access control.
        /// </summary>
        /// <returns>Returns sharepoint access token.</returns>
        Task<string> AcquireSPTokenAsync();
    }
}