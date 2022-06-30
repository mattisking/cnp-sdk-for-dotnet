using System.Collections.Generic;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using Moq;
using Cnp.Sdk.Interfaces;
using System.Net.Http;
using Cnp.Sdk.Core;

namespace Cnp.Sdk.Test.Functional
{
    [TestFixture]
    internal class TestActivate
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
        public void SimpleActivate()
        {
            var activate = new activate
            {
                id = "1",
                reportGroup = "Planets",
                orderId = "12344",
                amount = 1500,
                orderSource = orderSourceType.ecommerce,
                card = new giftCardCardType
                {
                    type = methodOfPaymentTypeEnum.GC,
                    number = "414100000000000000",
                    cardValidationNum = "123",
                    expDate = "1215"
                }
            };
            var response = _cnp.Activate(activate);
            Assert.AreEqual("000", response.response);
            Assert.AreEqual("sandbox", response.location);
        }

        [Test]
        public void VirtualGiftCardActivate()
        {
            var activate = new activate
            {
                id = "1",
                reportGroup = "Planets",
                orderId = "12344",
                amount = 1500,
                orderSource = orderSourceType.ecommerce,
                virtualGiftCard = new virtualGiftCardType
                {
                    accountNumberLength = 123,
                    giftCardBin = "123"
                }
            };

            var response = _cnp.Activate(activate);
            Assert.AreEqual("000", response.response);
        }
    }
}
