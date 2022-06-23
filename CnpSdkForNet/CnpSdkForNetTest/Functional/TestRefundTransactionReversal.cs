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
    internal class TestRefundTransactionReversal
    {
        private CnpOnline _cnp;
        private Mock<ILogger> _mockLogger;
        private ICommunications _communications;

        [OneTimeSetUp]
        public void SetUpCnp()
        {
            _mockLogger = new Mock<ILogger>();

            var configManager = new ConfigManager();
            var config = configManager.getConfig();
            var handler = new CommunicationsHttpClientHandler(config);
            _communications = new Communications(new HttpClient(handler), config);

            _cnp = new CnpOnline(_communications, config, _mockLogger.Object);
        }

        [Test]
        public void SimpleTransactionReversal()
        {
            var reversal = new refundTransactionReversal
            {
                id = "1",
                reportGroup = "Planets",
                cnpTxnId = 12345678000L,
                amount = 106,
            };

            var response = _cnp.RefundTransactionReversal(reversal);
            Assert.AreEqual("Approved", response.message);
        }

        [Test]
        public void TestTransactionReversalHandleSpecialCharacters()
        {
            var reversal = new refundTransactionReversal()
            {
                id = "1",
                reportGroup = "Planets",
                cnpTxnId = 12345678000L,
                amount = 106,
                customerId = "<'&\">"
            };


            var response = _cnp.RefundTransactionReversal(reversal);
            Assert.AreEqual("Approved", response.message);
        }

        [Test]
        public void TestTransactionReversalAsync()
        {
            var reversal = new refundTransactionReversal
            {
                id = "1",
                reportGroup = "Planets",
                cnpTxnId = 12345678000L,
                amount = 106,
                customerId = "<'&\">"
            };

            CancellationToken cancellationToken = new CancellationToken(false);
            var response = _cnp.RefundTransactionReversalAsync(reversal, cancellationToken);
            Assert.AreEqual("000", response.Result.response);
        }
        
        [Test]
        public void TestSimpleTransactionReversalWithLocation()
        {
            var reversal = new refundTransactionReversal
            {
                id = "1",
                reportGroup = "Planets",
                cnpTxnId = 12345678000L,
                amount = 106,
            };

            var response = _cnp.RefundTransactionReversal(reversal);
            Assert.AreEqual("sandbox", response.location);
            Assert.AreEqual("Approved", response.message);
        }
        
        [Test]
        public void TestTransactionReversalWithRecycling()
        {
            var reversal = new refundTransactionReversal
            {
                id = "1",
                reportGroup = "Planets",
                cnpTxnId = 12345678000L,
                amount = 106,
            };

            var response = _cnp.RefundTransactionReversal(reversal);
            Assert.AreEqual("sandbox", response.location);
            Assert.AreEqual("Approved", response.message);
            Assert.AreEqual(12345678000L, response.recyclingResponse.creditCnpTxnId);
        }
    }
}
