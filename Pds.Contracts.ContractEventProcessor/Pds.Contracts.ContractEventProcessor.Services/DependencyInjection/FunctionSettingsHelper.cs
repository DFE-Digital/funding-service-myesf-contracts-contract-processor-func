using Microsoft.Extensions.Configuration;
using Pds.Contracts.ContractEventProcessor.Services.Interfaces;
using Pds.Contracts.ContractEventProcessor.Services.Models;

namespace Pds.Contracts.ContractEventProcessor.Services.DependencyInjection
{
    /// <summary>
    /// Setting options helper.
    /// </summary>
    public class FunctionSettingsHelper
    {
        /// <summary>
        /// Bind settings from configuration for injection purposes.
        /// </summary>
        /// <param name="configuration">Configuration options.</param>
        /// <returns>Returns an instance of FunctionSettings.</returns>
        public static IFunctionSettings GetSettings(IConfiguration configuration)
        {
            FunctionSettings functionSettings = new FunctionSettings();
            configuration.GetSection("Values").Bind(functionSettings);

            return functionSettings;
        }
    }
}