using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Pds.Contracts.ContractEventProcessor.Services.Configurations;
using Pds.Contracts.ContractEventProcessor.Services.DocumentServices;
using Pds.Contracts.ContractEventProcessor.Services.Implementations;
using Pds.Contracts.ContractEventProcessor.Services.Interfaces;
using Pds.Contracts.ContractEventProcessor.Services.SharePointClient;
using Pds.Contracts.Data.Api.Client.Registrations;

namespace Pds.Contracts.ContractEventProcessor.Services.DependencyInjection
{
    /// <summary>
    /// Extensions class for <see cref="IServiceCollection"/> for registering the feature's services.
    /// </summary>
    public static class FeatureServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services for the current feature to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the feature's services to.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> provider.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddFeatureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register config
            services.AddSingleton(s => FunctionSettingsHelper.GetSettings(configuration));

            // Register contract data api client
            var policyRegistry = services.AddPolicyRegistry();
            services.AddContractsDataApiClient(configuration, policyRegistry);

            // Register event processor workflow
            services.AddScoped<IContractEventSessionManager, ContractEventSessionManager>();
            services.AddSingleton<IWorkflowStateManager, WorkflowStateManager>();
            services.AddScoped<IContractService, ContractService>();

            // Register custom logger
            services.AddScoped<IContractEventProcessLog, ContractEventProcessLog>();
            services.AddScoped(typeof(IContractEventProcessorLogger<>), typeof(ContractEventProcessorLogger<>));

            // Register contract event services
            services.AddScoped<IContractCreationService, ContractCreationService>();
            services.AddScoped<IContractApprovalService, ContractApprovalService>();
            services.AddScoped<IContractWithdrawService, ContractWithdrawService>();

            // Register sharepoint related services
            services.Configure<SPClientServiceConfiguration>(options => configuration.GetSection(nameof(SPClientServiceConfiguration)).Bind(options));
            services.AddSharePointClientContext();
            if (configuration.GetValue<bool>("FeatureFlag:UseSPAzureADAuthentication"))
            {
                services.AddSingleton(graphClient => new GraphServiceClient(new ClientSecretCredential(
                configuration.GetValue<string>("SPClientServiceConfiguration:TenantId"),
                configuration.GetValue<string>("SPClientServiceConfiguration:AADClientId"),
                configuration.GetValue<string>("SPClientServiceConfiguration:AADClientSecret"))));

                services.AddScoped<ISharePointClientService, SPGraphClientService>();
            }
            else
            {
                services.AddScoped<ISharePointClientService, SharePointClientService>();
            }

            services.AddSingleton<IContractEventMapper, ContractEventMapper>();

            // Register pdf contract document related services
            services.AddAsposeLicense();
            services.AddScoped<IDocumentManagementService, AsposeDocumentManagementService>();

            return services;
        }
    }
}