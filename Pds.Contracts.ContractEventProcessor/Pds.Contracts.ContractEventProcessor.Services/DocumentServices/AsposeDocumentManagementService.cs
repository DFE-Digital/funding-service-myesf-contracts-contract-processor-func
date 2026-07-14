using Aspose.Pdf;
using Pds.Contracts.ContractEventProcessor.Services.Extensions;
using Pds.Contracts.ContractEventProcessor.Services.Interfaces;
using System.Diagnostics;
using System.IO;

namespace Pds.Contracts.ContractEventProcessor.Services.DocumentServices
{
    /// <summary>
    /// Aspose implementation of the IDocumentManagementService.
    /// </summary>
    public class AsposeDocumentManagementService : IDocumentManagementService
    {
        private readonly IContractEventProcessorLogger<AsposeDocumentManagementService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsposeDocumentManagementService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public AsposeDocumentManagementService(IContractEventProcessorLogger<AsposeDocumentManagementService> logger)
        {
            _logger = logger;
        }

        #region IDocumentManagementService Implementation

        /// <summary>
        /// Converts the byte array PDF to be in the PDF\2A_2B format.
        /// </summary>
        /// <param name="pdf">The original PDF.</param>
        /// <returns>The formatted PDF.</returns>
        public byte[] ConvertToPdfA(byte[] pdf)
        {
            _logger.LogInformation("Starting conversion of PDF to PDF_A_2B.");
            var sw = Stopwatch.StartNew();

            using (var inputStream = new MemoryStream(pdf))
            {
                using (var doc = new Document(inputStream))
                {
                    doc.ConvertToPdfA();

                    using (var outputStream = new MemoryStream())
                    {
                        EnsureLandscapePagesAreMarkedCorrectly(doc);
                        doc.Save(outputStream);
                        _logger.LogInformation($"PDF conversion took {sw.ElapsedMilliseconds}ms to complete.");
                        return outputStream.ToArray();
                    }
                }
            }
        }

        #endregion IDocumentManagementService Implementation


        #region Addition Helpers

        /// <summary>
        /// EnsureLandscapePagesAreMarkedCorrectly.
        /// </summary>
        /// <param name="document">document.</param>
        protected void EnsureLandscapePagesAreMarkedCorrectly(Document document)
        {
            foreach (Page page in document.Pages)
            {
                if (page.Rect.Width > page.Rect.Height)
                {
                    page.PageInfo.IsLandscape = true;
                }
            }
        }

        #endregion Addition Helpers
    }
}