﻿using System.Collections.Generic;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using Moq;
using Cnp.Sdk.Interfaces;
using Cnp.Sdk.Core;
using System.Net.Http;

namespace Cnp.Sdk.Test.Functional
{
    [TestFixture]
    internal class TestUnload
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
        public void SimpleUnload()
        {
            var unload = new unload
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

            var response = _cnp.Unload(unload);
            Assert.AreEqual("000", response.response);
            Assert.AreEqual("sandbox", response.location);
        }

    }
}
