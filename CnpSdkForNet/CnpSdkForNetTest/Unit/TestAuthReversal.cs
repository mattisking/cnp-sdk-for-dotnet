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
    class TestAuthReversal
    {
        private CnpOnline _cnp;
        private Mock<ILogger> _mockLogger;
        private Mock<ICommunications> _mockCommunications;

        [OneTimeSetUp]
        public void SetUpCnp()
        {
            _mockLogger = new Mock<ILogger>();
            _mockCommunications = new Mock<ICommunications>();

            _cnp = new CnpOnline(_mockCommunications.Object, _mockLogger.Object);
        }

        [Test]
        public void TestSurchargeAmount()
        {
            authReversal reversal = new authReversal();
            reversal.cnpTxnId = 3;
            reversal.amount = 2;
            reversal.surchargeAmount = 1;
            reversal.payPalNotes = "note";
            reversal.reportGroup = "Planets";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<amount>2</amount>\r\n<surchargeAmount>1</surchargeAmount>\r\n<payPalNotes>note</payPalNotes>.*", RegexOptions.Singleline) ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><authReversalResponse><cnpTxnId>123</cnpTxnId></authReversalResponse></cnpOnlineResponse>");

            _cnp.AuthReversal(reversal);
        }

        [Test]
        public void TestSurchargeAmount_Optional()
        {
            authReversal reversal = new authReversal();
            reversal.cnpTxnId = 3;
            reversal.amount = 2;
            reversal.payPalNotes = "note";
            reversal.reportGroup = "Planets";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<amount>2</amount>\r\n<payPalNotes>note</payPalNotes>.*", RegexOptions.Singleline) ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><authReversalResponse><cnpTxnId>123</cnpTxnId></authReversalResponse></cnpOnlineResponse>");

            _cnp.AuthReversal(reversal);
        }
        
        [Test]
        public void TestAuthReversalWithLocation()
        {
            authReversal reversal = new authReversal();
            reversal.cnpTxnId = 3;
            reversal.amount = 2;
            reversal.surchargeAmount = 1;
            reversal.payPalNotes = "note";
            reversal.reportGroup = "Planets";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<amount>2</amount>\r\n<surchargeAmount>1</surchargeAmount>\r\n<payPalNotes>note</payPalNotes>.*", RegexOptions.Singleline)))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><authReversalResponse><cnpTxnId>123</cnpTxnId><location>sandbox</location></authReversalResponse></cnpOnlineResponse>");

            var response = _cnp.AuthReversal(reversal);
            
            Assert.NotNull(response);
            Assert.AreEqual("sandbox", response.location);
        }

    }
}
