using System.Collections.Generic;
using NUnit.Framework;
using System.Threading;
using Microsoft.Extensions.Logging;
using Moq;
using Cnp.Sdk.Interfaces;
using System.Net.Http;
using Cnp.Sdk.Core;

namespace Cnp.Sdk.Test.Functional
{
    [TestFixture]
    internal class TestSubmerchant
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
        public void SubmerchantCredit()
        {
            var submerchantCredit = new submerchantCredit
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
                submerchantName = "Vantiv",
                customIdentifier = "WorldPay"
            };

            var response = _cnp.SubmerchantCredit(submerchantCredit);
            Assert.AreEqual("sandbox", response.location);
            Assert.AreEqual("000", response.response);
        }

        [Test]
        public void TestSubmerchantCreditAsync()
        {
            var submerchantCredit = new submerchantCredit
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
                submerchantName = "Vantiv",
                customIdentifier = "WorldPay"
            };

            CancellationToken cancellationToken = new CancellationToken(false);
            var response = _cnp.SubmerchantCreditAsync(submerchantCredit, cancellationToken);
            Assert.AreEqual("000", response.Result.response);
        }

        [Test]
        public void SubmerchantDebit()
        {
            var submerchantDebit = new submerchantDebit
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
                submerchantName = "Vantiv",
                customIdentifier = "WorldPay"
            };

            var response = _cnp.SubmerchantDebit(submerchantDebit);
            Assert.AreEqual("000", response.response);
        }

        [Test]
        public void TestSubmerchantDebitAsync()
        {
            var submerchantDebit = new submerchantDebit
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
                submerchantName = "Vantiv",
                customIdentifier = "WorldPay"
            };

            CancellationToken cancellationToken = new CancellationToken(false);
            var response = _cnp.SubmerchantDebitAsync(submerchantDebit, cancellationToken);
            Assert.AreEqual("000", response.Result.response);
        }
    }
}
