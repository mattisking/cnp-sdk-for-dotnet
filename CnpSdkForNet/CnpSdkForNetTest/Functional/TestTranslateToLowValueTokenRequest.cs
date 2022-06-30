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
    internal class TestTranslateToLowValueTokenRequest
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
        public void SimpleTranslateToLowValueTokenRequest()
        {

            var query = new translateToLowValueTokenRequest
            {
                id = "1",
                reportGroup = "Planets",
                orderId = "2121",
                token = "822",
            };

            var response = _cnp.TranslateToLowValueTokenRequest(query);
            var queryResponse = (translateToLowValueTokenResponse)response;

            Assert.NotNull(queryResponse);
            Assert.AreEqual("sandbox", queryResponse.location);
            Assert.AreEqual("822", queryResponse.response);

        }


        [Test]
        public void SimpleTranslateToLowValueTokenRequestWithDiffToken()
        {

            var query = new translateToLowValueTokenRequest
            {
                id = "1",
                reportGroup = "Planets",
                orderId = "2121",
                token = "821",
            };

            var response = _cnp.TranslateToLowValueTokenRequest(query);
            var queryResponse = (translateToLowValueTokenResponse)response;

            Assert.NotNull(queryResponse);
            Assert.AreEqual("821", queryResponse.response);

        }

        [Test]
        public void SimpleTranslateToLowValueTokenRequestWithDefaultToken()
        {

            var query = new translateToLowValueTokenRequest
            {
                id = "1",
                reportGroup = "Planets",
                orderId = "2121",
                token = "55",
            };

            var response = _cnp.TranslateToLowValueTokenRequest(query);
            var queryResponse = (translateToLowValueTokenResponse)response;

            Assert.NotNull(queryResponse);
            Assert.AreEqual("803", queryResponse.response);

        }

        [Test]
        public void TestTranslateToLowValueTokenRequestAsync()
        {

            var query = new translateToLowValueTokenRequest
            {
                id = "1",
                reportGroup = "Planets",
                orderId = "2121",
                token = "822",
            };

            CancellationToken cancellationToken = new CancellationToken(false);
            var response = _cnp.TranslateToLowValueTokenRequestAsync(query, cancellationToken);
            var queryResponse = (translateToLowValueTokenResponse)response.Result;

            Assert.NotNull(queryResponse);
            Assert.AreEqual("822", queryResponse.response);

        }

    }
}
