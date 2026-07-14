using Microsoft.Extensions.Options;
using Microsoft.SharePoint.Client;
using Pds.Contracts.ContractEventProcessor.Services.Configurations;
using Pds.Contracts.ContractEventProcessor.Services.CustomExceptionHandlers;
using Pds.Contracts.ContractEventProcessor.Services.Extensions;
using Pds.Contracts.ContractEventProcessor.Services.Interfaces;
using System;
using System.IO;
using System.Resources;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pds.Contracts.ContractEventProcessor.Services.SharePointClient
{
    /// <summary>
    /// The SharePoint Client Service.
    /// </summary>
    public class SharePointClientService : ISharePointClientService
    {
        /// <summary>
        /// Gets the embeded resources namespace.
        /// </summary>
        /// <value>
        /// The embeded resources namespace.
        /// </value>
        internal static string EmbededResourcesNamespace => "Pds.Contracts.ContractEventProcessor.Services.DocumentServices.Resources.ContractPdf";

        private static string TestContractPdfFileName => $"{EmbededResourcesNamespace}.12345678_Test_v1.pdf";

        private readonly IContractEventProcessorLogger<ISharePointClientService> _logger;
        private readonly ClientContext _clientContext;
        private readonly SPClientServiceConfiguration _spConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharePointClientService"/> class.
        /// </summary>
        /// <param name="logger">ILogger reference to log output.</param>
        /// <param name="clientContext">The SharePoint Client Context.</param>
        /// <param name="spClientServiceConfiguration">The SharePoint Client Service configuration.</param>
        public SharePointClientService(
            IContractEventProcessorLogger<ISharePointClientService> logger,
            ClientContext clientContext,
            IOptions<SPClientServiceConfiguration> spClientServiceConfiguration)
        {
            _logger = logger;
            _clientContext = clientContext;
            _spConfig = spClientServiceConfiguration.Value;
        }

        /// <summary>
        /// Returns the document stream for the given file name in specified library.
        /// </summary>
        /// <param name="filename">The document to find and stream.</param>
        /// <param name="libraryName">The folder name of the document location.</param>
        /// <returns> A stream containing the contents of the document. </returns>
        public async Task<byte[]> GetDocument(string filename, string libraryName)
        {
            _logger.LogInformation($"[{nameof(GetDocument)}] - Attempting to connect to SharePoint location.");

            libraryName = GetUrlSafeFolderNameForContractDocument(libraryName);

            string fileRelativeUrl = $"{_spConfig.RelativeSiteURL}/{libraryName}/{filename}";

            _logger.LogInformation($"[{nameof(GetDocument)}] - Connecting to SharePoint with fileRelativeUrl: ${fileRelativeUrl}");

            try
            {
                var file = _clientContext.Web.GetFileByServerRelativeUrl(fileRelativeUrl);
                _clientContext.Load(file);
                if (file is null)
                {
                    _logger.LogError($"[{nameof(GetDocument)}] - File not found: {fileRelativeUrl}");
                    return HandleFileNotFoundExceptionWithTestPdf(new DocumentNotFoundException($"[{nameof(GetDocument)}] - File not found: {fileRelativeUrl}"));
                }

                ClientResult<Stream> stream = file.OpenBinaryStream();
                await _clientContext.ExecuteQueryAsync();

                if (stream.Value is null)
                {
                    _logger.LogError($"[{nameof(GetDocument)}] - File not stream from location: {fileRelativeUrl}");
                    return HandleFileNotFoundExceptionWithTestPdf(new DocumentNotFoundException($"[{nameof(GetDocument)}] - File not found: {fileRelativeUrl}"));
                }

                _logger.LogInformation($"[{nameof(GetDocument)}] - File stream location: {fileRelativeUrl} completed.");
                return stream.Value.ToByteArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{nameof(GetDocument)}] - The contract pdf file is not accessible. File: {fileRelativeUrl}");
                return HandleFileNotFoundExceptionWithTestPdf(new DocumentNotAccessibleException("The contract pdf file is not accessible.", ex));
            }
        }

        private string GetUrlSafeFolderNameForContractDocument(string folderName)
        {
            Regex pattern = new Regex("[-,+]");
            folderName = pattern.Replace(folderName, string.Empty);
            return System.Web.HttpUtility.UrlPathEncode(folderName);
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
                using var stream = typeof(SharePointClientService).Assembly.GetManifestResourceStream(TestContractPdfFileName);
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