using System.Collections.Generic;
using NUnit.Framework;
using System.Threading;
using Microsoft.Extensions.Logging;
using Moq;
using Cnp.Sdk.Interfaces;
using Cnp.Sdk.Core;
using System.Net.Http;

namespace Cnp.Sdk.Test.Functional
{
    [TestFixture]
    internal class TestVendor
    {
        private CnpOnline _cnp;
        private Mock<ILogger<CnpOnline>> _mockLogger;
        private Mock<ILogger<Communications>> _mockComLogger;
        private ICommunications _communications;

        [OneTimeSetUp]
        public void SetUpCnp()
        {
            _mockLogger = new Mock<ILogger<CnpOnline>>();
            _mockComLogger = new Mock<ILogger<Communications>>();

            var configManager = new ConfigManager();
            var config = configManager.getConfig();
            var handler = new CommunicationsHttpClientHandler(config);
            _communications = new Communications(new HttpClient(handler), _mockComLogger.Object, config);

            _cnp = new CnpOnline(_communications, config, _mockLogger.Object);
        }

        [Test]
        public void VendorCredit()
        {
            var vendorCredit = new vendorCredit
            {
                // attributes.
                id = "1",
                reportGroup = "Default Report Group",
                // required child elements.
                accountInfo = new echeckType()
                {
                    accType = echeckAccountTypeEnum.Savings,
                    accNum = "1234",
                    routingNum = "12345678"
                },
                amount = 1512l,
                fundingSubmerchantId = "value for fundingSubmerchantId",
                fundsTransferId = "value for fundsTransferId",
                vendorName = "Vantiv"
            };

            var response = _cnp.VendorCredit(vendorCredit);
            Assert.AreEqual("000", response.response);
            Assert.AreEqual("sandbox", response.location);
        }

        [Test]
        public void VendorCreditWithVendorAddress()
        {
            var vendorCredit = new vendorCredit
            {
                // attributes.
                id = "1",
                reportGroup = "Default Report Group",
                // required child elements.
                accountInfo = new echeckType()
                {
                    accType = echeckAccountTypeEnum.Savings,
                    accNum = "1234",
                    routingNum = "12345678"
                },
                vendorAddress = new addressType()
                {
                    addressLine1 = "37 Main Street",
                    addressLine2 = "",
                    addressLine3 = "",
                    city = "Augusta",
                    state = "Wisconsin",
                    zip = "28209",
                    country = countryTypeEnum.USA
                },
                amount = 1512l,
                fundingSubmerchantId = "value for fundingSubmerchantId",
                fundsTransferId = "value for fundsTransferId",
                vendorName = "Vantiv"
            };

            var response = _cnp.VendorCredit(vendorCredit);
            Assert.AreEqual("000", response.response);
            Assert.AreEqual("sandbox", response.location);
        }

        [Test]
        public void VendorCreditWithFundingCustomerId()
        {
            var vendorCredit = new vendorCredit
            {
                // attributes.
                id = "1",
                reportGroup = "Default Report Group",
                // required child elements.
                accountInfo = new echeckType()
                {
                    accType = echeckAccountTypeEnum.Savings,
                    accNum = "1234",
                    routingNum = "12345678"
                },
                amount = 1512l,
                fundingCustomerId = "value for fundingCustomerId",
                fundsTransferId = "value for fundsTransferId",
                vendorName = "Vantiv"
            };

            var response = _cnp.VendorCredit(vendorCredit);
            Assert.AreEqual("000", response.response);
        }

        [Test]
        public void TestVendorCreditAsync()
        {
            var vendorCredit = new vendorCredit
            {
                // attributes.
                id = "1",
                reportGroup = "Default Report Group",
                // required child elements.
                accountInfo = new echeckType()
                {
                    accType = echeckAccountTypeEnum.Savings,
                    accNum = "1234",
                    routingNum = "12345678"
                },
                amount = 1512l,
                fundingSubmerchantId = "value for fundingSubmerchantId",
                fundsTransferId = "value for fundsTransferId",
                vendorName = "Vantiv"
            };

            CancellationToken cancellationToken = new CancellationToken(false);
            var response = _cnp.VendorCreditAsync(vendorCredit, cancellationToken);
            Assert.AreEqual("000", response.Result.response);
            Assert.AreEqual("sandbox", response.Result.location);
        }

        [Test]
        public void ReserveDebit()
        {
            var vendorDebit = new vendorDebit
            {
                // attributes.
                id = "1",
                reportGroup = "Default Report Group",
                // required child elements.
                accountInfo = new echeckType()
                {
                    accType = echeckAccountTypeEnum.Savings,
                    accNum = "1234",
                    routingNum = "12345678"
                },
                amount = 1512l,
                fundingSubmerchantId = "value for fundingSubmerchantId",
                fundsTransferId = "value for fundsTransferId",
                vendorName = "WorldPay"
            };

            var response = _cnp.VendorDebit(vendorDebit);
            Assert.AreEqual("000", response.response);
            Assert.AreEqual("sandbox", response.location);
        }

        [Test]
        public void TestVendorDebitWithVendorAddress()
        {
            var vendorDebit = new vendorDebit
            {
                // attributes.
                id = "1",
                reportGroup = "Default Report Group",
                // required child elements.
                accountInfo = new echeckType()
                {
                    accType = echeckAccountTypeEnum.Savings,
                    accNum = "1234",
                    routingNum = "12345678"
                },
                vendorAddress = new addressType()
                {
                    addressLine1 = "37 Main Street",
                    addressLine2 = "",
                    addressLine3 = "",
                    city = "Augusta",
                    state = "Wisconsin",
                    zip = "28209",
                    country = countryTypeEnum.USA
                },
                amount = 1512L,
                fundingSubmerchantId = "value for fundingSubmerchantId",
                fundsTransferId = "value for fundsTransferId",
                vendorName = "WorldPay"
            };

            var response = _cnp.VendorDebit(vendorDebit);
            Assert.AreEqual("000", response.response);
            Assert.AreEqual("sandbox", response.location);
        }

        [Test]
        public void ReserveDebitWithFundingCustomerId()
        {
            var vendorDebit = new vendorDebit
            {
                // attributes.
                id = "1",
                reportGroup = "Default Report Group",
                // required child elements.
                accountInfo = new echeckType()
                {
                    accType = echeckAccountTypeEnum.Savings,
                    accNum = "1234",
                    routingNum = "12345678"
                },
                amount = 1512l,
                fundingSubmerchantId = "value for fundingSubmerchantId",
                fundsTransferId = "value for fundsTransferId",
                fundingCustomerId = "value for fundingCustomerId",
                vendorName = "WorldPay"
            };

            var response = _cnp.VendorDebit(vendorDebit);
            Assert.AreEqual("000", response.response);
        }

        [Test]
        public void TestReserveDebitAsync()
        {
            var vendorDebit = new vendorDebit
            {
                // attributes.
                id = "1",
                reportGroup = "Default Report Group",
                // required child elements.
                accountInfo = new echeckType()
                {
                    accType = echeckAccountTypeEnum.Savings,
                    accNum = "1234",
                    routingNum = "12345678"
                },
                amount = 1512l,
                fundingSubmerchantId = "value for fundingSubmerchantId",
                fundsTransferId = "value for fundsTransferId",
                vendorName = "WorldPay"
            };

            CancellationToken cancellationToken = new CancellationToken(false);
            var response = _cnp.VendorDebitAsync(vendorDebit, cancellationToken);
            Assert.AreEqual("000", response.Result.response);
            Assert.AreEqual("sandbox", response.Result.location);
        }
    }
}
