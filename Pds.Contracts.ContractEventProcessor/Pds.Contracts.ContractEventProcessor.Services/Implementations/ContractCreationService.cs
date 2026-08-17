using Microsoft.Extensions.Options;
using Pds.Contracts.ContractEventProcessor.Services.Configurations;
using Pds.Contracts.ContractEventProcessor.Services.CustomExceptionHandlers;
using Pds.Contracts.ContractEventProcessor.Services.DocumentServices;
using Pds.Contracts.ContractEventProcessor.Services.Extensions;
using Pds.Contracts.ContractEventProcessor.Services.Interfaces;
using Pds.Contracts.ContractEventProcessor.Services.Models;
using Pds.Contracts.Data.Api.Client.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Pds.Contracts.ContractEventProcessor.Services.Implementations
{
    /// <summary>
    /// The contract creation service.
    /// </summary>
    public class ContractCreationService : IContractCreationService
    {
        private readonly IContractEventProcessorLogger<IContractCreationService> _logger;
        private readonly IContractsDataService _contractsDataService;
        private readonly ISharePointClientService _sharePointClientService;
        private readonly IDocumentManagementService _documentManagementService;
        private readonly IContractEventMapper _contractEventMapper;
        private readonly SPClientServiceConfiguration _spConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractCreationService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="contractsDataService">The contracts data service.</param>
        /// <param name="sharePointClientService">The share point client service.</param>
        /// <param name="documentManagementService">The Aspose pdf document management service.</param>
        /// <param name="contractEventMapper">The contract processor service.</param>
        /// <param name="spClientServiceConfiguration">The SharePoint Client Service configuration.</param>
        public ContractCreationService(
            IContractEventProcessorLogger<IContractCreationService> logger,
            IContractsDataService contractsDataService,
            ISharePointClientService sharePointClientService,
            IDocumentManagementService documentManagementService,
            IContractEventMapper contractEventMapper,
            IOptions<SPClientServiceConfiguration> spClientServiceConfiguration)
        {
            _logger = logger;
            _contractsDataService = contractsDataService;
            _sharePointClientService = sharePointClientService;
            _documentManagementService = documentManagementService;
            _contractEventMapper = contractEventMapper;
            _spConfig = spClientServiceConfiguration.Value;
        }

        /// <summary>
        /// It will create a new contract.
        /// </summary>
        /// <param name="contractEvent">New contract Data.</param>
        /// <returns>Returns true when a new contract created else false.</returns>
        public async Task<bool> CreateAsync(ContractEvent contractEvent)
        {
            _logger.LogInformation($"[{nameof(CreateAsync)}] - Processing message for contract creation. ContractNumber: {contractEvent.ContractNumber}, ContractVersion: {contractEvent.ContractVersion}");

            var createRequest = _contractEventMapper.GetCreateRequest(contractEvent);
            var fileName = _contractEventMapper.GetFileNameForContractDocument(contractEvent.Ukprn, contractEvent.ContractNumber, contractEvent.ContractVersion);
            var folderName = _contractEventMapper.GetFolderNameForContractDocument(contractEvent.FundingType.GetEnumShortName(), contractEvent.ContractPeriodValue, _spConfig.PublicationFolderSuffix);
            var pdfDoc = await _sharePointClientService.GetDocument(fileName, folderName);
            var pdfADoc = _documentManagementService.ConvertToPdfA(pdfDoc);
            createRequest.ContractContent = _contractEventMapper.GetContractContent(pdfADoc, fileName);
            await _contractsDataService.CreateContractAsync(createRequest);
            return true;
        }
    }
}