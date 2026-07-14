using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions.Store;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Contracts.ContractEventProcessor.Services.Configurations;
using Pds.Contracts.ContractEventProcessor.Services.CustomExceptionHandlers;
using Pds.Contracts.ContractEventProcessor.Services.SharePointClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pds.Contracts.ContractEventProcessor.Services.Tests.Unit
{
    [TestClass, TestCategory("Unit")]
    public class SPGraphClientServiceTests
    {
        private readonly ILogger<SPGraphClientService> _mockLogger
          = Mock.Of<ILogger<SPGraphClientService>>(MockBehavior.Strict);

        private readonly IOptions<SPClientServiceConfiguration> _mockSPClientServiceConfiguration
            = Mock.Of<IOptions<SPClientServiceConfiguration>>(MockBehavior.Strict);

        private readonly IRequestAdapter _mockRequestAdapter = Mock.Of<IRequestAdapter>();

        private Mock<GraphServiceClient> _mockGraphServiceClient;

        [TestMethod]
        public async Task GetDocument_ExpectedResultAsync()
        {
            //Arrange
            SetMockSetup_Config();
            SetMockSetup_Logger(LogLevel.Information);
            SetMockSetup_GraphSite();
            SetMockSetup_GraphDrive();

            var documentLibraryName = "CTEC2526OutputDocuments%20for%20Publication";
            var fileName = "12345678_TEC-1001_v1.pdf";
            var expectedfileContent = "Sample document";
            var mockStream = new MemoryStream(Encoding.UTF8.GetBytes(expectedfileContent));

            Mock.Get(_mockRequestAdapter)
            .Setup(adapter => adapter.SendPrimitiveAsync<Stream>(It.Is<RequestInformation>(info => info.HttpMethod == Method.GET && info.URI.ToString().Contains("/content")), It.IsAny<Dictionary<string, ParsableFactory<IParsable>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockStream);

            _mockGraphServiceClient = new Mock<GraphServiceClient>(_mockRequestAdapter, null);
            var spGraphClientService = new SPGraphClientService(_mockGraphServiceClient.Object, _mockSPClientServiceConfiguration, _mockLogger);

            //Act
            var fileContent = await spGraphClientService.GetDocument(fileName, documentLibraryName);


            //Assert
            Assert.AreEqual(expectedfileContent, Encoding.UTF8.GetString(fileContent));
            Mock.Get(_mockSPClientServiceConfiguration).VerifyAll();
            Mock.Get(_mockLogger).VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(DocumentNotAccessibleException))]
        public async Task GetDocument_WhenGraphSite_ReturnsNull_ThrowsExcetion()
        {
            //Arrange
            SetMockSetup_Config();
            SetMockSetup_Logger(LogLevel.Information);
            SetMockSetup_Logger(LogLevel.Error);

            var documentLibraryName = "CTEC2526OutputDocuments%20for%20Publication";
            var fileName = "12345678_TEC-1001_v1.pdf";

            _mockGraphServiceClient = new Mock<GraphServiceClient>(_mockRequestAdapter, null);
            var spGraphClientService = new SPGraphClientService(_mockGraphServiceClient.Object, _mockSPClientServiceConfiguration, _mockLogger);

            //Act
            await spGraphClientService.GetDocument(fileName, documentLibraryName);
        }

        [TestMethod]
        [ExpectedException(typeof(DocumentNotAccessibleException))]
        public async Task GetDocument_WhenGraphDrives_ReturnsNull_ThrowsExcetion()
        {
            //Arrange
            SetMockSetup_Config();
            SetMockSetup_Logger(LogLevel.Information);
            SetMockSetup_Logger(LogLevel.Error);
            SetMockSetup_GraphSite();

            var mockResponse = new DriveCollectionResponse()
            {
                Value = new List<Drive>()
            };

            Mock.Get(_mockRequestAdapter)
            .Setup(adapter => adapter.SendAsync(It.Is<RequestInformation>(info => info.HttpMethod == Method.GET), DriveCollectionResponse.CreateFromDiscriminatorValue, It.IsAny<Dictionary<string, ParsableFactory<IParsable>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

            var documentLibraryName = "CTEC2526OutputDocuments%20for%20Publication";
            var fileName = "12345678_TEC-1001_v1.pdf";

            _mockGraphServiceClient = new Mock<GraphServiceClient>(_mockRequestAdapter, null);
            var spGraphClientService = new SPGraphClientService(_mockGraphServiceClient.Object, _mockSPClientServiceConfiguration, _mockLogger);

            //Act
            await spGraphClientService.GetDocument(fileName, documentLibraryName);
        }

        [TestMethod]
        [ExpectedException(typeof(DocumentNotAccessibleException))]
        public async Task GetDocument_WhenGraphDrives_WithSpecificDrive_ReturnsNull_ThrowsExcetion()
        {
            //Arrange
            SetMockSetup_Config();
            SetMockSetup_Logger(LogLevel.Information);
            SetMockSetup_Logger(LogLevel.Error);
            SetMockSetup_GraphSite();
            SetMockSetup_GraphDrive("Test");

            var documentLibraryName = "CTEC2526OutputDocuments%20for%20Publication";
            var fileName = "12345678_TEC-1001_v1.pdf";

            _mockGraphServiceClient = new Mock<GraphServiceClient>(_mockRequestAdapter, null);
            var spGraphClientService = new SPGraphClientService(_mockGraphServiceClient.Object, _mockSPClientServiceConfiguration, _mockLogger);

            //Act
            await spGraphClientService.GetDocument(fileName, documentLibraryName);
        }

        [TestMethod]
        [ExpectedException(typeof(DocumentNotAccessibleException))]
        public async Task GetDocument_WhenGraphFileContents_ReturnsNull_ThrowsExcetion()
        {
            //Arrange
            SetMockSetup_Config();
            SetMockSetup_Logger(LogLevel.Information);
            SetMockSetup_Logger(LogLevel.Error);
            SetMockSetup_GraphSite();
            SetMockSetup_GraphDrive();
            var documentLibraryName = "CTEC2526OutputDocuments%20for%20Publication";
            var fileName = "12345678_TEC-1001_v1.pdf";

            _mockGraphServiceClient = new Mock<GraphServiceClient>(_mockRequestAdapter, null);
            var spGraphClientService = new SPGraphClientService(_mockGraphServiceClient.Object, _mockSPClientServiceConfiguration, _mockLogger);

            //Act
            await spGraphClientService.GetDocument(fileName, documentLibraryName);
        }

        private void SetMockSetup_Config()
        {
            var spClientServiceConfiguration = GetSPClientServiceConfiguration();
            Mock.Get(_mockSPClientServiceConfiguration)
               .Setup(x => x.Value)
               .Returns(spClientServiceConfiguration)
               .Verifiable();
        }

        private void SetMockSetup_Logger(LogLevel logLevel)
        {
            Mock.Get(_mockLogger)
            .Setup(logger => logger.Log(
            It.Is<LogLevel>(l => l == logLevel),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
        }

        private void SetMockSetup_GraphSite()
        {
            Mock.Get(_mockRequestAdapter).SetupGet(adapter => adapter.BaseUrl).Returns("http://graph.test.internal/mock");
            Mock.Get(_mockRequestAdapter).SetupSet(adapter => adapter.BaseUrl = It.IsAny<string>());
            Mock.Get(_mockRequestAdapter).Setup(adapter => adapter.EnableBackingStore(It.IsAny<IBackingStoreFactory>()));

            var mockSite = new Site
            {
                Id = "testgovuk.sharepoint.com,2C712604,2D2244C3",
                DisplayName = "Test Site",
                WebUrl = "testgovuk.sharepoint.com"
            };

            Mock.Get(_mockRequestAdapter)
              .Setup(adapter => adapter.SendAsync(It.Is<RequestInformation>(info => info.HttpMethod == Method.GET), Site.CreateFromDiscriminatorValue, It.IsAny<Dictionary<string, ParsableFactory<IParsable>>>(), It.IsAny<CancellationToken>()))
              .ReturnsAsync(mockSite);
        }

        private void SetMockSetup_GraphDrive(string driveName = "CTEC2526OutputDocuments for Publication")
        {
            var mockResponse = new DriveCollectionResponse
            {
                Value = new List<Drive>
                        {
                            new Drive { Id = "drive-1", Name = driveName },
                            new Drive { Id = "drive-2", Name = "Test Drive" }
                        }
            };

            Mock.Get(_mockRequestAdapter)
            .Setup(adapter => adapter.SendAsync(It.Is<RequestInformation>(info => info.HttpMethod == Method.GET), DriveCollectionResponse.CreateFromDiscriminatorValue, It.IsAny<Dictionary<string, ParsableFactory<IParsable>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);
        }

        private SPClientServiceConfiguration GetSPClientServiceConfiguration()
        {
            return new SPClientServiceConfiguration()
            {
                ApiBaseAddress = "https://testgovuk.sharepoint.com",
                AppUri = "testgovuk.sharepoint.com",
                AADClientId = "fasdfasdfasdfasdfasdf",
                AADClientSecret = "asdfasdfasdfasdfasdf",
                PublicationFolderSuffix = "Outputfolder",
                RelativeSiteURL = "/sites/pdstest",
                Resource = "0000003456345600000000000",
                TenantId = "3423wertwet234542345"
            };
        }
    }
}