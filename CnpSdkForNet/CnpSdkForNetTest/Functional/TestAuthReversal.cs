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
    internal class TestAuthReversal
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
        public void SimpleAuthReversal()
        {
            var reversal = new authReversal
            {
                id = "1",
                reportGroup = "Planets",
                cnpTxnId = 12345678000L,
                amount = 106,
                payPalNotes = "Notes"
            };

            var response = _cnp.AuthReversal(reversal);
            Assert.AreEqual("Approved", response.message);
        }

        [Test]
        public void TestAuthReversalHandleSpecialCharacters()
        {
            var reversal = new authReversal
            {
                id = "1",
                reportGroup = "Planets",
                cnpTxnId = 12345678000L,
                amount = 106,
                payPalNotes = "<'&\">"
            };


            var response = _cnp.AuthReversal(reversal);
            Assert.AreEqual("Approved", response.message);
        }

        [Test]
        public void TestAuthReversalAsync()
        {
            var reversal = new authReversal
            {
                id = "1",
                reportGroup = "Planets",
                cnpTxnId = 12345678000L,
                amount = 106,
                payPalNotes = "<'&\">"
            };

            CancellationToken cancellationToken = new CancellationToken(false);
            var response = _cnp.AuthReversalAsync(reversal, cancellationToken);
            Assert.AreEqual("000", response.Result.response);
        }
        
        [Test]
        public void SimpleAuthReversalWithLocation()
        {
            var reversal = new authReversal
            {
                id = "1",
                reportGroup = "Planets",
                cnpTxnId = 12345678000L,
                amount = 106,
                payPalNotes = "Notes"
            };

            var response = _cnp.AuthReversal(reversal);
            Assert.AreEqual("sandbox", response.location);
            Assert.AreEqual("Approved", response.message);
        }

        [Test]
        public void TestAuthReversalAsync_newMerchantId()
        {
            _cnp.SetMerchantId("1234");
            var reversal = new authReversal
            {
                id = "1",
                reportGroup = "Planets",
                cnpTxnId = 12345678000L,
                amount = 106,
                payPalNotes = "<'&\">"
            };

            CancellationToken cancellationToken = new CancellationToken(false);
            var response = _cnp.AuthReversalAsync(reversal, cancellationToken);
            Assert.AreEqual("000", response.Result.response);
        }
    }
}
