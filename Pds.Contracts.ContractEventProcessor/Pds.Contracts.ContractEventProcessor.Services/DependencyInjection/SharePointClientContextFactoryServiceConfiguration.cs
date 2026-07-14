using Microsoft.Extensions.DependencyInjection;
using Microsoft.SharePoint.Client;
using Pds.Contracts.ContractEventProcessor.Services.Interfaces;
using Pds.Contracts.ContractEventProcessor.Services.SharePointClient;

namespace Pds.Contracts.ContractEventProcessor.Services.DependencyInjection
{
    /// <summary>
    /// The SharePoint Client Context Factory Service Configuration.
    /// </summary>
    public static class SharePointClientContextFactoryServiceConfiguration
    {
        /// <summary>
        /// Add SharePoint Context Factory to service collection.
        /// </summary>
        /// <param name="serviceCollection">The ServiceCollection.</param>
        /// <returns>Returns ServiceCollection.</returns>
        public static IServiceCollection AddSharePointContextFactory(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ISPClientContextFactory, SPClientContextFactory>();
            serviceCollection.AddHttpClient<ISPAuthenticationTokenService, SPAuthenticationTokenService>();
            return serviceCollection;
        }

        /// <summary>
        /// Add SharePoint Client Context.
        /// </summary>
        /// <param name="serviceCollection">The ServiceCollection.</param>
        /// <returns>Returns ServiceCollection.</returns>
        public static IServiceCollection AddSharePointClientContext(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSharePointContextFactory();

            serviceCollection.AddScoped<ClientContext>((services) =>
            {
                var clientContextFactory = services.GetService<ISPClientContextFactory>();
                return clientContextFactory.GetSPClientContext();
            });

            return serviceCollection;
        }
    }
}