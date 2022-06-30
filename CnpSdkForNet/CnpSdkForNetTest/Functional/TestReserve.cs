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
    internal class TestReserve
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
        public void ReserveCredit()
        {
            var reserveCredit = new reserveCredit
            {
                // attributes.
                id = "1",
                reportGroup = "Default Report Group",
                // required child elements.
                amount = 1512l,
                fundingSubmerchantId = "value for fundingSubmerchantId",
                fundsTransferId = "value for fundsTransferId"
            };

            var response = _cnp.ReserveCredit(reserveCredit);
            Assert.AreEqual("sandbox", response.location);
            Assert.AreEqual("000", response.response);
        }

        [Test]
        public void TestReserveCreditAsync()
        {
            var reserveCredit = new reserveCredit
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
            var response = _cnp.ReserveCreditAsync(reserveCredit, cancellationToken);
            Assert.AreEqual("000", response.Result.response);
        }
        
        [Test]
        public void ReserveCreditFundingCustomerId()
        {
            var reserveCredit = new reserveCredit
            {
                // attributes.
                id = "1",
                reportGroup = "Default Report Group",
                // required child elements.
                amount = 1512l,
                fundingCustomerId = "value for fundingCustomerId",
                fundsTransferId = "value for fundsTransferId"
            };

            var response = _cnp.ReserveCredit(reserveCredit);
            Assert.AreEqual("000", response.response);
        }

        [Test]
        public void ReserveDebit()
        {
            var reserveDebit = new reserveDebit
            {
                // attributes.
                id = "1",
                reportGroup = "Planets",
                // required child elements.
                amount = 1512l,
                fundingSubmerchantId = "value for fundingSubmerchantId",
                fundsTransferId = "value for fundsTransferId"
            };

            var response = _cnp.ReserveDebit(reserveDebit);
            Assert.AreEqual("000", response.response);
        }

        [Test]
        public void TestReserveDebitAsync()
        {
            var reserveDebit = new reserveDebit
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
            var response = _cnp.ReserveDebitAsync(reserveDebit, cancellationToken);
            Assert.AreEqual("000", response.Result.response);
        }
        
        [Test]
        public void ReserveDebitFundingCustomerId()
        {
            var reserveDebit = new reserveDebit
            {
                // attributes.
                id = "1",
                reportGroup = "Default Report Group",
                // required child elements.
                amount = 1512l,
                fundingCustomerId = "value for fundingCustomerId",
                fundsTransferId = "value for fundsTransferId"
            };

            var response = _cnp.ReserveDebit(reserveDebit);
            Assert.AreEqual("000", response.response);
        }
        
        [Test]
        public void ReserveDebitXMLCharacters()
        {
            var reserveDebit = new reserveDebit
            {
                // attributes.
                id = "1",
                reportGroup = "Default Report Group",
                // required child elements.
                amount = 1512l,
                fundingCustomerId = "value <for> fundingCustomerId",
                fundsTransferId = "value for fundsTransferId"
            };

            var response = _cnp.ReserveDebit(reserveDebit);
            Assert.AreEqual("000", response.response);
        }
    }
}
