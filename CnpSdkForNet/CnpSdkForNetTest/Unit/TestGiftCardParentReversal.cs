﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Cnp.Sdk.Interfaces;

namespace Cnp.Sdk.Test.Unit
{

    [TestFixture]
    class TestGiftCardParentReversal
    {
        private CnpOnline _cnp;
        private Mock<ILogger<CnpOnline>> _mockLogger;
        private Mock<ICommunications> _mockCommunications;

        [OneTimeSetUp]
        public void SetUpCnp()
        {
            _mockLogger = new Mock<ILogger<CnpOnline>>();
            _mockCommunications = new Mock<ICommunications>();

            _cnp = new CnpOnline(_mockCommunications.Object, _mockLogger.Object);
        }

        [Test]
        public void DepositReversal()
        {
            depositReversal reversal = new depositReversal();
            reversal.id = "1";
            reversal.reportGroup = "planets";
            reversal.cnpTxnId = 123456000;
            giftCardCardType card = new giftCardCardType();
            card.type = methodOfPaymentTypeEnum.GC;
            card.number = "414100000000000000";
            card.expDate = "1210";
            card.pin = "1234";
            reversal.card = card;
            reversal.originalRefCode = "123";
            reversal.originalAmount = 123;
            reversal.originalTxnTime = new DateTime(2017, 01, 01);
            reversal.originalSystemTraceId = 123;
            reversal.originalSequenceNumber = "123456";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<cnpTxnId>123456000</cnpTxnId>\r\n<card>\r\n<type>GC</type>\r\n<number>414100000000000000</number>\r\n<expDate>1210</expDate>\r\n<pin>1234</pin>\r\n</card>\r\n<originalRefCode>123</originalRefCode>\r\n<originalAmount>123</originalAmount>\r\n<originalTxnTime>2017-01-01T00:00:00Z</originalTxnTime>\r\n<originalSystemTraceId>123</originalSystemTraceId>\r\n<originalSequenceNumber>123456</originalSequenceNumber>.*", RegexOptions.Singleline)  ))
                    .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><depositReversalResponse><cnpTxnId>123</cnpTxnId></depositReversalResponse></cnpOnlineResponse>");

            _cnp.DepositReversal(reversal);
        }

        [Test]
        public void RefundReversal()
        {
            refundReversal reversal = new refundReversal();
            reversal.id = "1";
            reversal.reportGroup = "planets";
            reversal.cnpTxnId = 123456000;
            giftCardCardType card = new giftCardCardType();
            card.type = methodOfPaymentTypeEnum.GC;
            card.number = "414100000000000000";
            card.expDate = "1210";
            card.pin = "1234";
            reversal.card = card;
            reversal.originalRefCode = "123";
            reversal.originalAmount = 123;
            reversal.originalTxnTime = new DateTime(2017, 01, 01);
            reversal.originalSystemTraceId = 123;
            reversal.originalSequenceNumber = "123456";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<cnpTxnId>123456000</cnpTxnId>\r\n<card>\r\n<type>GC</type>\r\n<number>414100000000000000</number>\r\n<expDate>1210</expDate>\r\n<pin>1234</pin>\r\n</card>\r\n<originalRefCode>123</originalRefCode>\r\n<originalAmount>123</originalAmount>\r\n<originalTxnTime>2017-01-01T00:00:00Z</originalTxnTime>\r\n<originalSystemTraceId>123</originalSystemTraceId>\r\n<originalSequenceNumber>123456</originalSequenceNumber>.*", RegexOptions.Singleline)  ))
                    .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><refundReversalResponse><cnpTxnId>123</cnpTxnId><location>sandbox</location></refundReversalResponse></cnpOnlineResponse>");

            var response = _cnp.RefundReversal(reversal);
            
            Assert.AreEqual("sandbox", response.location);
        }

        [Test]
        public void ActivateReversal()
        {
            activateReversal reversal = new activateReversal();
            reversal.id = "1";
            reversal.reportGroup = "planets";
            reversal.cnpTxnId = 123456000;
            reversal.virtualGiftCardBin = "123";
            giftCardCardType card = new giftCardCardType();
            card.type = methodOfPaymentTypeEnum.GC;
            card.number = "414100000000000000";
            card.expDate = "1210";
            card.pin = "1234";
            reversal.card = card;
            reversal.originalRefCode = "123";
            reversal.originalAmount = 123;
            reversal.originalTxnTime = new DateTime(2017, 01, 01);
            reversal.originalSystemTraceId = 123;
            reversal.originalSequenceNumber = "123456";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<cnpTxnId>123456000</cnpTxnId>\r\n<virtualGiftCardBin>123</virtualGiftCardBin>\r\n<card>\r\n<type>GC</type>\r\n<number>414100000000000000</number>\r\n<expDate>1210</expDate>\r\n<pin>1234</pin>\r\n</card>\r\n<originalRefCode>123</originalRefCode>\r\n<originalAmount>123</originalAmount>\r\n<originalTxnTime>2017-01-01T00:00:00Z</originalTxnTime>\r\n<originalSystemTraceId>123</originalSystemTraceId>\r\n<originalSequenceNumber>123456</originalSequenceNumber>.*", RegexOptions.Singleline)  ))
                    .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><activateReversalResponse><cnpTxnId>123</cnpTxnId><location>sandbox</location></activateReversalResponse></cnpOnlineResponse>");

            var response = _cnp.ActivateReversal(reversal);
            
            Assert.NotNull(response);
            Assert.AreEqual("sandbox", response.location);
        }

        [Test]
        public void DeactivateReversal()
        {
            deactivateReversal reversal = new deactivateReversal();
            reversal.id = "1";
            reversal.reportGroup = "planets";
            reversal.cnpTxnId = 123456000;
            giftCardCardType card = new giftCardCardType();
            card.type = methodOfPaymentTypeEnum.GC;
            card.number = "414100000000000000";
            card.expDate = "1210";
            card.pin = "1234";
            reversal.card = card;
            reversal.originalRefCode = "123";
            reversal.originalTxnTime = new DateTime(2017, 01, 01);
            reversal.originalSystemTraceId = 123;
            reversal.originalSequenceNumber = "123456";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<cnpTxnId>123456000</cnpTxnId>\r\n<card>\r\n<type>GC</type>\r\n<number>414100000000000000</number>\r\n<expDate>1210</expDate>\r\n<pin>1234</pin>\r\n</card>\r\n<originalRefCode>123</originalRefCode>\r\n<originalTxnTime>2017-01-01T00:00:00Z</originalTxnTime>\r\n<originalSystemTraceId>123</originalSystemTraceId>\r\n<originalSequenceNumber>123456</originalSequenceNumber>.*", RegexOptions.Singleline)  ))
                    .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><deactivateReversalResponse><cnpTxnId>123</cnpTxnId><location>sandbox</location></deactivateReversalResponse></cnpOnlineResponse>");

            var response = _cnp.DeactivateReversal(reversal);
            
            Assert.NotNull(response);
            Assert.AreEqual("sandbox", response.location);
        }

        [Test]
        public void LoadReversal()
        {
            loadReversal reversal = new loadReversal();
            reversal.id = "1";
            reversal.reportGroup = "planets";
            reversal.cnpTxnId = 123456000;
            giftCardCardType card = new giftCardCardType();
            card.type = methodOfPaymentTypeEnum.GC;
            card.number = "414100000000000000";
            card.expDate = "1210";
            card.pin = "1234";
            reversal.card = card;
            reversal.originalRefCode = "123";
            reversal.originalAmount = 123;
            reversal.originalTxnTime = new DateTime(2017, 01, 01);
            reversal.originalSystemTraceId = 123;
            reversal.originalSequenceNumber = "123456";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<cnpTxnId>123456000</cnpTxnId>\r\n<card>\r\n<type>GC</type>\r\n<number>414100000000000000</number>\r\n<expDate>1210</expDate>\r\n<pin>1234</pin>\r\n</card>\r\n<originalRefCode>123</originalRefCode>\r\n<originalAmount>123</originalAmount>\r\n<originalTxnTime>2017-01-01T00:00:00Z</originalTxnTime>\r\n<originalSystemTraceId>123</originalSystemTraceId>\r\n<originalSequenceNumber>123456</originalSequenceNumber>.*", RegexOptions.Singleline)  ))
                    .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><loadReversalResponse><cnpTxnId>123</cnpTxnId><location>sandbox</location></loadReversalResponse></cnpOnlineResponse>");

            var response = _cnp.LoadReversal(reversal);
            
            Assert.NotNull(response);
            Assert.AreEqual("sandbox", response.location);
        }

        [Test]
        public void UnloadReversal()
        {
            unloadReversal reversal = new unloadReversal();
            reversal.id = "1";
            reversal.reportGroup = "planets";
            reversal.cnpTxnId = 123456000;
            giftCardCardType card = new giftCardCardType();
            card.type = methodOfPaymentTypeEnum.GC;
            card.number = "414100000000000000";
            card.expDate = "1210";
            card.pin = "1234";
            reversal.card = card;
            reversal.originalRefCode = "123";
            reversal.originalAmount = 123;
            reversal.originalTxnTime = new DateTime(2017, 01, 01);
            reversal.originalSystemTraceId = 123;
            reversal.originalSequenceNumber = "123456";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<cnpTxnId>123456000</cnpTxnId>\r\n<card>\r\n<type>GC</type>\r\n<number>414100000000000000</number>\r\n<expDate>1210</expDate>\r\n<pin>1234</pin>\r\n</card>\r\n<originalRefCode>123</originalRefCode>\r\n<originalAmount>123</originalAmount>\r\n<originalTxnTime>2017-01-01T00:00:00Z</originalTxnTime>\r\n<originalSystemTraceId>123</originalSystemTraceId>\r\n<originalSequenceNumber>123456</originalSequenceNumber>.*", RegexOptions.Singleline)  ))
                    .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><unloadReversalResponse><cnpTxnId>123</cnpTxnId><location>sandbox</location></unloadReversalResponse></cnpOnlineResponse>");

            var response = _cnp.UnloadReversal(reversal);
            
            Assert.NotNull(response);
            Assert.AreEqual("sandbox", response.location);
        }
    }
}
