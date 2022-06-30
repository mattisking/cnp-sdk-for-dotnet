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
    internal class TestPayFac
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
        public void PayFacCredit()
        {
            var payFacCredit = new payFacCredit
            {
                // attributes.
                id = "1",
                reportGroup = "Default Report Group",
                // required child elements.
                amount = 1512l,
                fundingSubmerchantId = "value for fundingSubmerchantId",
                fundsTransferId = "value for fundsTransferId"
            };

            var response = _cnp.PayFacCredit(payFacCredit);
            Assert.AreEqual("000", response.response);
            Assert.AreEqual("sandbox", response.location);
        }

        [Test]
        public void TestPayFacCreditAsync()
        {
            var payFacCredit = new payFacCredit
            {
                // attributes.
                id = "1",
                reportGroup = "Default Report Group",
                // required child elements.
                amount = 1512l,
                fundingSubmerchantId = "value for fundingSubmerchantId",
                fundsTransferId = "value for fundsTransferId"
            };

            CancellationToken cancellationToken = new CancellationToken(false);
            var response = _cnp.PayFacCreditAsync(payFacCredit, cancellationToken);
            Assert.AreEqual("000", response.Result.response);
        }

        [Test]
        public void PayFacDebit()
        {
            var payFacDebit = new payFacDebit
            {
                // attributes.
                id = "1",
                reportGroup = "Planets",
                // required child elements.
                amount = 1512l,
                fundingSubmerchantId = "value for fundingSubmerchantId",
                fundsTransferId = "value for fundsTransferId"
            };

            var response = _cnp.PayFacDebit(payFacDebit);
            Assert.AreEqual("000", response.response);
            Assert.AreEqual("sandbox", response.location);
        }

        [Test]
        public void TestPayFacDebitAsync()
        {
            var payFacDebit = new payFacDebit
            {
                // attributes.
                id = "1",
                reportGroup = "Planets",
                // required child elements.
                amount = 1512l,
                fundingSubmerchantId = "value for fundingSubmerchantId",
                fundsTransferId = "value for fundsTransferId"
            };

            CancellationToken cancellationToken = new CancellationToken(false);
            var response = _cnp.PayFacDebitAsync(payFacDebit, cancellationToken);
            Assert.AreEqual("000", response.Result.response);
            Assert.AreEqual("sandbox", response.Result.location);
        }
    }
}
