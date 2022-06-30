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
    internal class TestEcheckRedeposit
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
        public void simpleEcheckRedeposit()
        {
            var echeckredeposit = new echeckRedeposit
            {
                id = "1",
                reportGroup = "Planets",
                cnpTxnId = 123456
            };

            var response = _cnp.EcheckRedeposit(echeckredeposit);
            Assert.AreEqual("Approved", response.message);
        }
        
        [Test]
        public void simpleEcheckRedepositWithLocation()
        {
            var echeckredeposit = new echeckRedeposit
            {
                id = "1",
                reportGroup = "Planets",
                cnpTxnId = 123456
            };

            var response = _cnp.EcheckRedeposit(echeckredeposit);
            Assert.AreEqual("sandbox", response.location);
            Assert.AreEqual("Approved", response.message);
        }

        [Test]
        public void EcheckRedepositWithEcheck()
        {
            var echeckredeposit = new echeckRedeposit
            {
                id = "1",
                reportGroup = "Planets",
                cnpTxnId = 123456,
                echeck = new echeckType
                {
                    accType = echeckAccountTypeEnum.Checking,
                    accNum = "12345657890",
                    routingNum = "123456789",
                    checkNum = "123455",
                },

                customIdentifier = "CustomIdent"
            };
            
            var response = _cnp.EcheckRedeposit(echeckredeposit);
            Assert.AreEqual("Approved", response.message);
        }

        [Test]
        public void EcheckRedepositWithEcheckToken()
        {
            var echeckredeposit = new echeckRedeposit
            {
                id = "1",
                reportGroup = "Planets",
                cnpTxnId = 123456,
                token = new echeckTokenType
                {
                    accType = echeckAccountTypeEnum.Checking,
                    cnpToken = "1234565789012",
                    routingNum = "123456789",
                    checkNum = "123455"
                }
            };

            var response = _cnp.EcheckRedeposit(echeckredeposit);
            Assert.AreEqual("Approved", response.message);
        }

        [Test]
        public void TestEcheckRedepositAsync()
        {
            var echeckredeposit = new echeckRedeposit
            {
                id = "1",
                reportGroup = "Planets",
                cnpTxnId = 123456,
                token = new echeckTokenType
                {
                    accType = echeckAccountTypeEnum.Checking,
                    cnpToken = "1234565789012",
                    routingNum = "123456789",
                    checkNum = "123455"
                }
            };

            CancellationToken cancellationToken = new CancellationToken(false);
            var response = _cnp.EcheckRedepositAsync(echeckredeposit, cancellationToken);
            Assert.AreEqual("000", response.Result.response);
        }

    }
}
