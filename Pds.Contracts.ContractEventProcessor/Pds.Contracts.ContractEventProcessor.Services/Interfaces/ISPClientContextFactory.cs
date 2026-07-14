using Microsoft.SharePoint.Client;

namespace Pds.Contracts.ContractEventProcessor.Services.Interfaces
{
    /// <summary>
    /// SharePoint Client Context Service.
    /// </summary>
    public interface ISPClientContextFactory
    {
        /// <summary>
        /// Get sharepoint context.
        /// </summary>
        /// <returns>Returns the sharepoint context.</returns>
        ClientContext GetSPClientContext();
    }
}