using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Cnp.Sdk.Interfaces;

namespace Cnp.Sdk.Test.Unit
{
    [TestFixture]
    class TestCapture
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
        public void TestSimpleCapture()
        {
            capture capture = new capture();
            capture.cnpTxnId = 3;
            capture.amount = 2;
            capture.payPalNotes = "note";
            capture.reportGroup = "Planets";
            capture.pin = "1234";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<amount>2</amount>\r\n<payPalNotes>note</payPalNotes>\r\n<pin>1234</pin>.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><captureResponse><cnpTxnId>123</cnpTxnId></captureResponse></cnpOnlineResponse>");
            _cnp.Capture(capture);
        }

        [Test]
        public void TestSurchargeAmount()
        {
            capture capture = new capture();
            capture.cnpTxnId = 3;
            capture.amount = 2;
            capture.surchargeAmount = 1;
            capture.payPalNotes = "note";
            capture.reportGroup = "Planets";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<amount>2</amount>\r\n<surchargeAmount>1</surchargeAmount>\r\n<payPalNotes>note</payPalNotes>.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><captureResponse><cnpTxnId>123</cnpTxnId></captureResponse></cnpOnlineResponse>");

            _cnp.Capture(capture);
        }

        [Test]
        public void TestSurchargeAmount_Optional()
        {
            capture capture = new capture();
            capture.cnpTxnId = 3;
            capture.amount = 2;
            capture.payPalNotes = "note";
            capture.reportGroup = "Planets";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<amount>2</amount>\r\n<payPalNotes>note</payPalNotes>.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><captureResponse><cnpTxnId>123</cnpTxnId></captureResponse></cnpOnlineResponse>");

            _cnp.Capture(capture);
        }
        
        [Test]
        public void TestCaptureWithLocation()
        {
            capture capture = new capture();
            capture.cnpTxnId = 3;
            capture.amount = 2;
            capture.payPalNotes = "note";
            capture.reportGroup = "Planets";
            capture.pin = "1234";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<amount>2</amount>\r\n<payPalNotes>note</payPalNotes>\r\n<pin>1234</pin>.*", RegexOptions.Singleline)))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><captureResponse><cnpTxnId>123</cnpTxnId><location>sandbox</location></captureResponse></cnpOnlineResponse>");

            var response = _cnp.Capture(capture);
            
            Assert.NotNull(response);
            Assert.AreEqual("sandbox", response.location);
        }

    }
}
