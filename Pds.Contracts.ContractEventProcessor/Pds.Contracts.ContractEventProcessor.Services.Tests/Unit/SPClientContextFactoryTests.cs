using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Contracts.ContractEventProcessor.Services.Configurations;
using Pds.Contracts.ContractEventProcessor.Services.Interfaces;
using Pds.Contracts.ContractEventProcessor.Services.SharePointClient;
using Pds.Core.Utils.Interfaces;
using System;

namespace Pds.Contracts.ContractEventProcessor.Services.Tests.Unit
{
    [TestClass, TestCategory("Unit")]
    public class SPClientContextFactoryTests
    {
        private readonly IDateTimeProvider _mockDateTimeProvider
          = Mock.Of<IDateTimeProvider>(MockBehavior.Strict);

        private readonly ISPAuthenticationTokenService _mockSPAuthenticationTokenService
          = Mock.Of<ISPAuthenticationTokenService>(MockBehavior.Strict);

        private readonly ILogger<SPClientContextFactory> _mockLogger
           = Mock.Of<ILogger<SPClientContextFactory>>(MockBehavior.Strict);

        private readonly IOptions<SPClientServiceConfiguration> _mockSPClientServiceConfiguration
            = Mock.Of<IOptions<SPClientServiceConfiguration>>(MockBehavior.Strict);

        [TestMethod]
        public void GetSPClientContext_ExpectedResult()
        {
            //Arrange
            SetMockSetup_GetSPClientContext();
            var spClientContextFactory = new SPClientContextFactory(_mockDateTimeProvider, _mockSPAuthenticationTokenService, _mockLogger, _mockSPClientServiceConfiguration);

            //Act
            var result = spClientContextFactory.GetSPClientContext();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ClientContext>();
            Mock.Get(_mockSPAuthenticationTokenService)
                .Verify(x => x.AcquireSPTokenAsync(), Times.Never);
            Mock.Get(_mockSPClientServiceConfiguration).VerifyAll();
            Mock.Get(_mockLogger).VerifyAll();
        }

        private void SetMockSetup_GetSPClientContext()
        {
            var spClientServiceConfiguration = GetSPClientServiceConfiguration();
            var token = GetAccessToken();
            Mock.Get(_mockSPClientServiceConfiguration)
               .Setup(x => x.Value)
               .Returns(spClientServiceConfiguration)
               .Verifiable();

            Mock.Get(_mockSPAuthenticationTokenService)
              .Setup(x => x.AcquireSPTokenAsync())
              .ReturnsAsync(token)
              .Verifiable();

            SetMockSetup_Logger(LogLevel.Information);
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

        private string GetAccessToken()
        {
            return $"eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Im5PbzNaRHJPRFhFSzFqS1doWHNsSFJfS1hFZyIsImtpZCI6Im5PbzNaRHJPRFhFSzFqS1doWHNsSFJfS1hFZyJ9.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvdGVzdGVkdWNhdGlvbmdvdnVrLnNoYXJlcG9pbnQuY29tQDI2ZjVkYjM5LTQyNDYtNDBhYy1iNzJiLWY2MTdhOTgyOTcxMCIsImlzcyI6IjAwMDAwMDAxLTAwMDAtMDAwMC1jMDAwLTAwMDAwMDAwMDAwMEAyNmY1ZGIzOS00MjQ2LTQwYWMtYjcyYi1mNjE3YTk4Mjk3MTAiLCJpYXQiOjE2MTY0MzI2NDMsIm5iZiI6MTYxNjQzMjY0MywiZXhwIjoxNjE2NTE5MzQzLCJpZGVudGl0eXByb3ZpZGVyIjoiMDAwMDAwMDEtMDAwMC0wMDAwLWMwMDAtMDAwMDAwMDAwMDAwQDI2ZjVkYjM5LTQyNDYtNDBhYy1iNzJiLWY2MTdhOTgyOTcxMCIsIm5hbWVpZCI6ImU5OGYzZmMyLTYxNmItNDJmZC05ZjA5LTEyNDdiMmRmMTJjOEAyNmY1ZGIzOS00MjQ2LTQwYWMtYjcyYi1mNjE3YTk4Mjk3MTAiLCJvaWQiOiI3MmU4YjY1ZS0zZGRhLTQ2ZTQtYmVhYy1lOGMzZjIwNTEzNjAiLCJzdWIiOiI3MmU4YjY1ZS0zZGRhLTQ2ZTQtYmVhYy1lOGMzZjIwNTEzNjAiLCJ0cnVzdGVkZm9yZGVsZWdhdGlvbiI6ImZhbHNlIn0.eJr5FBVatib3jXewQ8nTnlEiVjK3b6f3e7c_SugIkGuBY-PNrukryy4mK48UShh20Qn1YUfnUsqEkup1ZsS_l153PmWy9h6ihx4YuNLU8lNElbbtaFl27LGYA4e07z2Xs643kNRvBYSgJDOGQc4-U0kJysGABZMExAr2ZI1jpEkbyy9kIdPgh_Ch-zto38EFs35H2xiXDg5zeftczTxX2RWDOa4Adta9f-LQs7bJcYFKy8RTS5s933ijz5OfG9K1eiyLVjV0fTQwKG-ZzNdRZuzxLRG9C2PLeKd-gQhjMTsvzp9-hE7K0WMguscR6QECW-mL-6JstKldbhB-tOm8xA";
        }
    }
}
