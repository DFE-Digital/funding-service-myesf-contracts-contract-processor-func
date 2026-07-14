using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pds.Contracts.ContractEventProcessor.Services.Enums;
using Pds.Contracts.ContractEventProcessor.Services.Extensions;

namespace Pds.Contracts.ContractEventProcessor.Services.Tests.Unit
{
    [TestClass, TestCategory("Unit")]
    public class EnumExtensionsTests
    {
        [TestMethod]
        public void GetEnumDisplayName_Test()
        {
            // Arrange
            string expected = "City Deal";
            var arrange = ContractFundingType.CityDeals;

            // Act
            var actual = arrange.GetEnumDisplayName();

            // Assert
            actual.Should().Be(expected);
        }

        [TestMethod]
        public void GetEnumShortName_Test()
        {
            // Arrange
            string expected = "CityDeals";
            var arrange = ContractFundingType.CityDeals;

            // Act
            var actual = arrange.GetEnumShortName();

            // Assert
            actual.Should().Be(expected);
        }

        [TestMethod]
        public void GetEnumShortName_ReturnsNullTest()
        {
            // Arrange
            var arrange = ContractFundingType.Unknown;

            // Act
            var actual = arrange.GetEnumShortName();

            // Assert
            actual.Should().BeNull();
        }
    }
}
