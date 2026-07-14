using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Pds.Contracts.ContractEventProcessor.Services.Configurations;
using Pds.Contracts.ContractEventProcessor.Services.CustomExceptionHandlers;
using Pds.Contracts.ContractEventProcessor.Services.Extensions;
using Pds.Contracts.ContractEventProcessor.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pds.Contracts.ContractEventProcessor.Services.SharePointClient
{
    /// <summary>
    /// The SharePoint Graph Client Service.
    /// </summary>
    public class SPGraphClientService : ISharePointClientService
    {
        /// <summary>
        /// Gets the embeded resources namespace.
        /// </summary>
        /// <value>
        /// The embeded resources namespace.
        /// </value>
        internal static string EmbededResourcesNamespace => "Pds.Contracts.ContractEventProcessor.Services.DocumentServices.Resources.ContractPdf";

        private static string TestContractPdfFileName => $"{EmbededResourcesNamespace}.12345678_Test_v1.pdf";

        private readonly ILogger<SPGraphClientService> _logger;
        private readonly SPClientServiceConfiguration _spConfig;
        private readonly GraphServiceClient _graphClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="SPGraphClientService"/> class.
        /// </summary>
        /// <param name="graphClient">The Graph Service Client.</param>
        /// <param name="logger">ILogger reference to log output.</param>
        /// <param name="spClientServiceConfiguration">The SharePoint Client Service configuration.</param>
        public SPGraphClientService(GraphServiceClient graphClient, IOptions<SPClientServiceConfiguration> spClientServiceConfiguration, ILogger<SPGraphClientService> logger)
        {
            _spConfig = spClientServiceConfiguration.Value;
            _logger = logger;
            _graphClient = graphClient;
        }

        /// <inheritdoc/>
        public async Task<byte[]> GetDocument(string filename, string libraryName)
        {
            _logger.LogInformation($"[{nameof(GetDocument)}] - Attempting to connect to SharePoint location using graph.");

            libraryName = Regex.Replace(libraryName, @"%20", " ");

            string fileRelativeUrl = $"{_spConfig.RelativeSiteURL}/{libraryName}/{filename}";

            try
            {
                _logger.LogInformation($"[{nameof(GetDocument)}] - Connecting to SharePoint using graph with fileRelativeUrl: ${fileRelativeUrl}");

                Site site = _graphClient
                            .Sites[_spConfig.AppUri + ":" + _spConfig.RelativeSiteURL]
                            .GetAsync().Result;

                if (site == null)
                {
                    _logger.LogError($"[{nameof(GetDocument)}] - SharePoint location not found: {_spConfig.RelativeSiteURL}");
                    return HandleFileNotFoundExceptionWithTestPdf(new DocumentNotFoundException($"[{nameof(GetDocument)}] - File not found: {fileRelativeUrl}"));
                }

                List<Drive> drives = await GetAllDrives(site.Id);

                if (drives.Count == 0)
                {
                    _logger.LogError($"[{nameof(GetDocument)}] - Failed to retrieve the sharepoint drives.");
                    return HandleFileNotFoundExceptionWithTestPdf(new DocumentNotFoundException($"[{nameof(GetDocument)}] - Failed to retrieve the sharepoint drives."));
                }

                Drive drive = drives?.FirstOrDefault(d => d.Name.Equals(libraryName, StringComparison.OrdinalIgnoreCase));

                if (drive == null)
                {
                    _logger.LogError($"[{nameof(GetDocument)}] - SharePoint library not found: {_spConfig.RelativeSiteURL + '/' + libraryName}");
                    return HandleFileNotFoundExceptionWithTestPdf(new DocumentNotFoundException($"[{nameof(GetDocument)}] - File not found: {fileRelativeUrl}"));
                }

                Stream fileContent = await _graphClient
                                        .Drives[drive.Id]
                                        .Root
                                        .ItemWithPath(filename)
                                        .Content
                                        .GetAsync();


                if (fileContent == null)
                {
                    _logger.LogError($"[{nameof(GetDocument)}] - File not found: {fileRelativeUrl}");
                    return HandleFileNotFoundExceptionWithTestPdf(new DocumentNotFoundException($"[{nameof(GetDocument)}] - File not found: {fileRelativeUrl}"));
                }

                using MemoryStream fileStreamContent = new MemoryStream();
                fileContent.CopyTo(fileStreamContent);

                _logger.LogInformation($"[{nameof(GetDocument)}] - using graph - File stream location: {fileRelativeUrl} completed.");

                return fileStreamContent.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{nameof(GetDocument)}] - The contract pdf file is not accessible. File: {fileRelativeUrl}");
                return HandleFileNotFoundExceptionWithTestPdf(new DocumentNotAccessibleException("The contract pdf file is not accessible.", ex));
            }
        }

        /// <summary>
        /// Returns all sharepoint drives.
        /// </summary>
        /// <param name="siteId">Graph Site Id.</param>
        /// <returns>List of drives.</returns>
        private async Task<List<Drive>> GetAllDrives(string siteId)
        {
            List<Drive> drives = new List<Drive>();

            var firstPage = await _graphClient
                .Sites[siteId]
                .Drives.GetAsync();

            PageIterator<Drive, DriveCollectionResponse> pageIterator = PageIterator<Drive, DriveCollectionResponse>.CreatePageIterator(_graphClient, firstPage, driveItem =>
            {
                drives.Add(driveItem);
                return true;
            });

            await pageIterator.IterateAsync();

            return drives;
        }

        private byte[] HandleFileNotFoundExceptionWithTestPdf<TException>(TException ex)
           where TException : Exception
        {
            if (_spConfig.ShouldErrorPdfNotFound)
            {
                throw ex;
            }
            else
            {
                using var stream = typeof(SPGraphClientService).Assembly.GetManifestResourceStream(TestContractPdfFileName);
                if (stream is null)
                {
                    throw new MissingManifestResourceException($"Failed to locate test contract PDF file ({TestContractPdfFileName}) in current assembly.");
                }
                else
                {
                    _logger.LogWarning($"[{nameof(HandleFileNotFoundExceptionWithTestPdf)}] - The contract pdf file have been replaced by the test contract pdf file.");
                    return stream.ToByteArray();
                }
            }
        }
    }
}
