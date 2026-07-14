using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Pds.Contracts.ContractEventProcessor.Services.Configurations;
using Pds.Contracts.ContractEventProcessor.Services.CustomExceptionHandlers;
using Pds.Contracts.ContractEventProcessor.Services.Interfaces;
using Pds.Contracts.ContractEventProcessor.Services.SharePointClient;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Pds.Contracts.ContractEventProcessor.Services.Tests.Unit
{
    [TestClass, TestCategory("Unit")]
    public class SPAuthenticationTokenServiceTests
    {
        private readonly IContractEventProcessorLogger<ISharePointClientService> _mockLogger
          = Mock.Of<IContractEventProcessorLogger<ISharePointClientService>>(MockBehavior.Strict);

        private readonly IOptions<SPClientServiceConfiguration> _mockSPClientServiceConfiguration
            = Mock.Of<IOptions<SPClientServiceConfiguration>>(MockBehavior.Strict);

        private readonly HttpClient _mockHttpClient
            = Mock.Of<HttpClient>(MockBehavior.Strict);

        [TestMethod]
        public async Task AcquireSPTokenAsync_ExpectedResult()
        {
            //Arrange
            SetMockSetup_AcquireSPTokenAsync();
            var handlerMock = new Mock<HttpMessageHandler>();
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(@"{ ""id"": 101, ""access_token"" : ""asdsdfgsdfg"" }"),
            };
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);
            var httpClient = new HttpClient(handlerMock.Object);

            var spAuthenticationTokenService = new SPAuthenticationTokenService(httpClient, _mockSPClientServiceConfiguration, _mockLogger);

            //Act
            await spAuthenticationTokenService.AcquireSPTokenAsync();

            //Assert
            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
               ItExpr.IsAny<CancellationToken>());
            Mock.Get(_mockSPClientServiceConfiguration).VerifyAll();
            Mock.Get(_mockLogger).VerifyAll();
        }

        [TestMethod]
        public void AcquireSPTokenAsync_ExpectedSPTokenAcquisitionFailureException()
        {
            //Arrange
            SetMockSetup_AcquireSPTokenAsync();
            SetMockSetup_Logger(LogLevel.Error);

            var handlerMock = new Mock<HttpMessageHandler>();
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(@"{ ""id"": 101, ""sdfg"" : ""asdsdfgsdfg"" }"),
            };
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);
            var httpClient = new HttpClient(handlerMock.Object);

            var spAuthenticationTokenService = new SPAuthenticationTokenService(httpClient, _mockSPClientServiceConfiguration, _mockLogger);

            //Act
            Func<Task> act = async () => await spAuthenticationTokenService.AcquireSPTokenAsync();

            //Assert
            act.Should().Throw<SPTokenAcquisitionFailureException>();
            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
               ItExpr.IsAny<CancellationToken>());
            Mock.Get(_mockSPClientServiceConfiguration).VerifyAll();
            Mock.Get(_mockLogger).VerifyAll();
        }

        private void SetMockSetup_AcquireSPTokenAsync()
        {
            // new StringContent(It.IsAny<string>(), Encoding.UTF8, "application/x-www-form-urlencoded")
            var spClientServiceConfiguration = GetSPClientServiceConfiguration();
            Mock.Get(_mockSPClientServiceConfiguration)
               .Setup(x => x.Value)
               .Returns(spClientServiceConfiguration)
               .Verifiable();

            SetMockSetup_Logger(LogLevel.Information);
        }

        private void SetMockSetup_Logger(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Information:
                    Mock.Get(_mockLogger).Setup(logger => logger.LogInformation(It.IsAny<string>()));
                    break;

                case LogLevel.Warning:
                    Mock.Get(_mockLogger).Setup(logger => logger.LogWarning(It.IsAny<string>()));
                    break;

                case LogLevel.Error:
                    Mock.Get(_mockLogger).Setup(logger => logger.LogError(It.IsAny<Exception>(), It.IsAny<string>()));
                    break;

                default:
                    break;
            }
        }

        private SPClientServiceConfiguration GetSPClientServiceConfiguration()
        {
            return new SPClientServiceConfiguration()
            {
                ApiBaseAddress = "https://testgovuk.sharepoint.com",
                AppUri = "00000034563456345600000000000/testgovuk.sharepoint.com",
                Authority = "https://accounts.accesscontrol.windows.net/",
                ClientId = "fasdfasdfasdfasdfasdf",
                ClientSecret = "asdfasdfasdfasdfasdf",
                PublicationFolderSuffix = "Outputfolder",
                RelativeSiteURL = "/sites/pdstest",
                Resource = "0000003456345600000000000",
                TenantId = "3423wertwet234542345"
            };
        }
    }
}