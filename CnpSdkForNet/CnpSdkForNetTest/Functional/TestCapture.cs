﻿using System.Collections.Generic;
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
    internal class TestCapture
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
        public void SimpleCapture()
        {
            var capture = new capture
            {
                id = "1",
                cnpTxnId = 123456000,
                amount = 106,
                payPalNotes = "Notes",
                pin = "1234"
            };

            var response = _cnp.Capture(capture);
            Assert.AreEqual("Approved", response.message);
        }

        [Test]
        public void SimpleCaptureWithPartial()
        {
            var capture = new capture
            {
                id = "1",
                cnpTxnId = 123456000,
                amount = 106,
                partial = true,
                payPalNotes = "Notes"
            };


            var response = _cnp.Capture(capture);
            Assert.AreEqual("Approved", response.message);
        }

        [Test]
        public void ComplexCapture()
        {
            var capture = new capture
            {
                id = "1",
                cnpTxnId = 123456000,
                amount = 106,
                payPalNotes = "Notes",
                pin = "1234",
                enhancedData = new enhancedData
                {
                    customerReference = "Cnp",
                    salesTax = 50,
                    deliveryType = enhancedDataDeliveryType.TBD
                },
                payPalOrderComplete = true,
                customBilling = new customBilling
                {
                    phone = "51312345678",
                    city = "Lowell",
                    url = "test.com",
                    descriptor = "Nothing",
                }
            };

            var response = _cnp.Capture(capture);
            Assert.AreEqual("Approved", response.message);
        }

        [Test]
        public void SimpleCaptureWithSpecial()
        {
            var capture = new capture
            {
                id = "1",
                cnpTxnId = 123456000,
                amount = 106,
                payPalNotes = "<'&\">"
            };
            
            var response = _cnp.Capture(capture);
            Assert.AreEqual("Approved", response.message);
        }

        [Test]
        public void SimpleCaptureWithLodgingInfo()
        {
            var capture = new capture
            {
                id = "1",
                cnpTxnId = 123456000,
                amount = 106,
                payPalNotes = "<'&\">",
                lodgingInfo = new lodgingInfo
                {
                    hotelFolioNumber = "12345",
                    checkInDate = new System.DateTime(2017, 1, 18),
                    customerServicePhone = "854213",
                    lodgingCharges = new List<lodgingCharge>(),

                }
            };
            capture.lodgingInfo.lodgingCharges.Add(new lodgingCharge() { name = lodgingExtraChargeEnum.GIFTSHOP });
            var response = _cnp.Capture(capture);
            Assert.AreEqual("Approved", response.message);
        }

        [Test]
        public void TestCaptureAsync()
        {
            var capture = new capture
            {
                id = "1",
                cnpTxnId = 123456000,
                amount = 106,
                payPalNotes = "<'&\">",
                lodgingInfo = new lodgingInfo
                {
                    hotelFolioNumber = "12345",
                    checkInDate = new System.DateTime(2017, 1, 18),
                    customerServicePhone = "854213",
                    lodgingCharges = new List<lodgingCharge>(),

                }
            };
            capture.lodgingInfo.lodgingCharges.Add(new lodgingCharge() { name = lodgingExtraChargeEnum.GIFTSHOP });
            CancellationToken cancellationToken = new CancellationToken(false);
            
            var response = _cnp.CaptureAsync(capture, cancellationToken);
            Assert.AreEqual("000", response.Result.response);
        }
        
        [Test]
        public void SimpleCaptureWithLocation()
        {
            var capture = new capture
            {
                id = "1",
                cnpTxnId = 123456000,
                amount = 106,
                payPalNotes = "Notes",
                pin = "1234"
            };

            var response = _cnp.Capture(capture);
            Assert.AreEqual("sandbox", response.location);
            Assert.AreEqual("Approved", response.message);
        }

        [Test]
        public void SimpleCaptureWithOptionalOrderID()
        {
            var capture = new capture
            {
                id = "1",
                cnpTxnId = 123456000,
                orderId = "orderidmorethan25charactersareacceptednow",
                amount = 106,
                payPalNotes = "Notes",
                pin = "1234"
            };

            var response = _cnp.Capture(capture);
            Assert.AreEqual("Approved", response.message);
        }
    }
}
