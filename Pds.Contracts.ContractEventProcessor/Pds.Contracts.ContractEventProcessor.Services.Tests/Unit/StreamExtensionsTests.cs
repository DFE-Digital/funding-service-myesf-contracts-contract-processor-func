using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pds.Contracts.ContractEventProcessor.Services.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pds.Contracts.ContractEventProcessor.Services.Tests.Unit
{
    [TestClass, TestCategory("Unit")]
    public class StreamExtensionsTests
    {
        [TestMethod]
        public void GetDocument_ExpectedResult()
        {
            //Arrange
            var expected = "Abc";
            var dummyStream = GetStreamFromString(expected);

            //Act
            var result = dummyStream.ToByteArray();

            //Assert
            string converted = GetStringFromByteArray(result);
            converted.Should().Be(expected);
        }

        private MemoryStream GetStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? string.Empty));
        }

        private string GetStringFromByteArray(byte[] value)
        {
            return Encoding.UTF8.GetString(value, 0, value.Length);
        }
    }
}
