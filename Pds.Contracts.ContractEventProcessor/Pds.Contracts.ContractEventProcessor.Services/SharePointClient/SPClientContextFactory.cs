using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SharePoint.Client;
using Pds.Contracts.ContractEventProcessor.Services.Configurations;
using Pds.Contracts.ContractEventProcessor.Services.Interfaces;
using Pds.Core.Utils.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;

namespace Pds.Contracts.ContractEventProcessor.Services.SharePointClient
{
    /// <inheritdoc/>
    public class SPClientContextFactory : ISPClientContextFactory
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ISPAuthenticationTokenService _spAuthenticationTokenService;
        private readonly ILogger<SPClientContextFactory> _logger;
        private readonly SPClientServiceConfiguration _spConfig;
        private JwtSecurityToken _spJwtSecurityToken;
        private string _spAccessToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="SPClientContextFactory"/> class.
        /// </summary>
        /// <param name="dateTimeProvider">The wrapper for DateTime utilities.</param>
        /// <param name="spAuthenticationTokenService">The SharePoint authentication token service.</param>
        /// <param name="logger">ILogger reference to log output.</param>
        /// <param name="spClientServiceConfiguration">The SharePoint Client Service configuration.</param>
        public SPClientContextFactory(IDateTimeProvider dateTimeProvider, ISPAuthenticationTokenService spAuthenticationTokenService, ILogger<SPClientContextFactory> logger, IOptions<SPClientServiceConfiguration> spClientServiceConfiguration)
        {
            _dateTimeProvider = dateTimeProvider;
            _spAuthenticationTokenService = spAuthenticationTokenService;
            _logger = logger;
            _spConfig = spClientServiceConfiguration.Value;
        }

        /// <inheritdoc/>
        public ClientContext GetSPClientContext()
        {
            Uri web = new Uri($"{_spConfig.ApiBaseAddress}{_spConfig.RelativeSiteURL}");

            _logger.LogInformation($"[{nameof(GetSPClientContext)}] - Attempting to get the SharePoint client context. Site: {web.AbsoluteUri}");

            ClientContext context = new ClientContext(web);
            context.ExecutingWebRequest += (sender, e) =>
            {
                string accessToken = EnsureAccessTokenAsync().GetAwaiter().GetResult();
                e.WebRequestExecutor.RequestHeaders["Authorization"] = "Bearer " + accessToken;
            };

            return context;
        }

        /// <summary>
        /// Get the SharePoint access token.
        /// </summary>
        /// <returns>Returns the SharePoint authentication token.</returns>
        private async Task<string> EnsureAccessTokenAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                _logger.LogInformation($"[{nameof(EnsureAccessTokenAsync)}] - Attempting to get the SharePoint Token.");

                if (string.IsNullOrEmpty(_spAccessToken) || _spJwtSecurityToken == null || _spJwtSecurityToken.ValidTo <= _dateTimeProvider.UtcNow())
                {
                    _spAccessToken = await _spAuthenticationTokenService.AcquireSPTokenAsync();
                    _spJwtSecurityToken = new JwtSecurityToken(_spAccessToken);
                }

                _logger.LogInformation($"[{nameof(EnsureAccessTokenAsync)}] - Acquired the SharePoint Token.");

                return _spAccessToken;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}