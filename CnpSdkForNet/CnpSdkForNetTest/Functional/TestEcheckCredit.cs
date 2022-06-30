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
    internal class TestEcheckCredit
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
        public void SimpleEcheckCredit()
        {
            var echeckcredit = new echeckCredit
            {
                id = "1",
                reportGroup = "Planets",
                amount = 12L,
                cnpTxnId = 123456789101112L,
            };

            var response = _cnp.EcheckCredit(echeckcredit);
            Assert.AreEqual("Approved", response.message);
            Assert.AreEqual("sandbox", response.location);
        }
        
        [Test]
        public void SimpleEcheckCreditWithLocation()
        {
            var echeckcredit = new echeckCredit
            {
                id = "1",
                reportGroup = "Planets",
                amount = 12L,
                cnpTxnId = 123456789101112L,
            };

            var response = _cnp.EcheckCredit(echeckcredit);
            Assert.AreEqual("sandbox", response.location);
            Assert.AreEqual("Approved", response.message);
        }

        [Test]
        public void NoCnpTxnId()
        {
            var echeckcredit = new echeckCredit
            {
                id = "1",
                reportGroup = "Planets",
            };

            try
            {
                _cnp.EcheckCredit(echeckcredit);
                Assert.Fail("Expected exception");
            }
            catch (CnpOnlineException e)
            {
                Assert.IsTrue(e.Message.Contains("Error validating xml data against the schema"));
            }
        }

        [Test]
        public void EcheckCreditWithEcheck()
        {
            var echeckcredit = new echeckCredit
            {
                id = "1",
                reportGroup = "Planets",
                amount = 12L,
                orderId = "12345",
                orderSource = orderSourceType.ecommerce,
                echeck = new echeckType
                {
                    accType = echeckAccountTypeEnum.Checking,
                    accNum = "12345657890",
                    routingNum = "123456789",
                    checkNum = "123455"
                },
                billToAddress = new contact
                {
                    name = "Bob",
                    city = "Lowell",
                    state = "MA",
                    email = "cnp.com",
                },
                customIdentifier = "CustomIdent"
            };

            var response = _cnp.EcheckCredit(echeckcredit);
            Assert.AreEqual("Approved", response.message);
        }

        [Test]
        public void EcheckCreditWithToken()
        {
            var echeckcredit = new echeckCredit
            {
                id = "1",
                reportGroup = "Planets",
                amount = 12L,
                orderId = "12345",
                orderSource = orderSourceType.ecommerce,
                echeckToken = new echeckTokenType
                {
                    accType = echeckAccountTypeEnum.Checking,
                    cnpToken = "1234565789012",
                    routingNum = "123456789",
                    checkNum = "123455",
                },

                billToAddress = new contact
                {
                    name = "Bob",
                    city = "Lowell",
                    state = "MA",
                    email = "cnp.com"
                }
            };



            var response = _cnp.EcheckCredit(echeckcredit);
            Assert.AreEqual("Approved", response.message);
        }

        [Test]
        public void MissingBilling()
        {
            var echeckcredit = new echeckCredit
            {
                id = "1",
                reportGroup = "Planets",
                amount = 12L,
                orderId = "12345",
                orderSource = orderSourceType.ecommerce,
                echeck = new echeckType
                {
                    accType = echeckAccountTypeEnum.Checking,
                    accNum = "12345657890",
                    routingNum = "123456789",
                    checkNum = "123455"
                }
            };

            try
            {
                _cnp.EcheckCredit(echeckcredit);
                Assert.Fail("Expected exception");
            }
            catch (CnpOnlineException e)
            {
                Assert.IsTrue(e.Message.Contains("Error validating xml data against the schema"));
            }
        }

        [Test]
        public void EcheckCreditWithSecondaryAmountWithOrderIdAndCcdPaymentInfo()
        {
            var echeckcredit = new echeckCredit
            {
                id = "1",
                reportGroup = "Planets",
                amount = 12L,
                secondaryAmount = 50,
                orderId = "12345",
                orderSource = orderSourceType.ecommerce,
                echeck = new echeckType
                {
                    accType = echeckAccountTypeEnum.Checking,
                    accNum = "12345657890",
                    routingNum = "123456789",
                    checkNum = "123455",
                    ccdPaymentInformation = "9876554"
                },
                billToAddress = new contact
                {
                    name = "Bob",
                    city = "Lowell",
                    state = "MA",
                    email = "cnp.com"

                }
            };

            var response = _cnp.EcheckCredit(echeckcredit);
            Assert.AreEqual("Approved", response.message);
        }

        [Test]
        public void EcheckCreditWithSecondaryAmountWithCnpTxnId()
        {
            var echeckcredit = new echeckCredit
            {
                id = "1",
                reportGroup = "Planets",
                amount = 12L,
                secondaryAmount = 50,
                cnpTxnId = 12345L,
                customIdentifier = "CustomIdent"
            };
            
            var response = _cnp.EcheckCredit(echeckcredit);
            Assert.AreEqual("Approved", response.message);
        }

        [Test]
        public void TestEcheckCreditAsync()
        {
            var echeckcredit = new echeckCredit
            {
                id = "1",
                reportGroup = "Planets",
                amount = 12L,
                cnpTxnId = 123456789101112L,
            };

            CancellationToken cancellationToken = new CancellationToken(false);
            var response = _cnp.EcheckCreditAsync(echeckcredit, cancellationToken);
            Assert.AreEqual("000", response.Result.response);
        }
    }
}
