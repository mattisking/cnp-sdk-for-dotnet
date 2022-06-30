using System.Collections.Generic;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using Moq;
using Cnp.Sdk.Interfaces;
using Cnp.Sdk.Core;
using System.Net.Http;

namespace Cnp.Sdk.Test.Functional
{
    [TestFixture]
    internal class TestHttpActionEvent
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
        public void TestHttpEvents()
        {
            var requestCount = 0;
            var responseCount = 0;
            var httpActionCount = 0;

            _cnp.HttpAction += (sender, args) =>
            {
                var eventArgs = (Communications.HttpActionEventArgs)args;
                httpActionCount++;
                if (eventArgs.RequestType == Communications.RequestType.Request)
                {
                    requestCount++;
                }
                else if (eventArgs.RequestType == Communications.RequestType.Response)
                {
                    responseCount++;
                }
            };

            var capture = new capture
            {
                cnpTxnId = 123456000,
                amount = 106,
                id = "1"
            };

            _cnp.Capture(capture);
            Assert.AreEqual(httpActionCount, 2);
            Assert.AreEqual(requestCount, 1);
            Assert.AreEqual(responseCount, 1);
        }
    }
}
