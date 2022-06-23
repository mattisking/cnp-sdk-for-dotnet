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
    class TestRefundTransactionReversal
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
            refundTransactionReversal reversal = new refundTransactionReversal();
            reversal.cnpTxnId = 3;
            reversal.amount = 2;
            reversal.surchargeAmount = 1;
            reversal.reportGroup = "Planets";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<amount>2</amount>\r\n<surchargeAmount>1</surchargeAmount>\r\n.*", RegexOptions.Singleline)))
                .Returns("<cnpOnlineResponse version='12.16' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><refundTransactionReversalResponse><cnpTxnId>3</cnpTxnId></refundTransactionReversalResponse></cnpOnlineResponse>");

            _cnp.RefundTransactionReversal(reversal);
        }

        [Test]
        public void TestSurchargeAmount_Optional()
        {
            refundTransactionReversal reversal = new refundTransactionReversal();
            reversal.cnpTxnId = 3;
            reversal.amount = 2;
            reversal.reportGroup = "Planets";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<amount>2</amount>\r\n.*", RegexOptions.Singleline)))
                .Returns("<cnpOnlineResponse version='12.16' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><refundTransactionReversalResponse><cnpTxnId>123</cnpTxnId></refundTransactionReversalResponse></cnpOnlineResponse>");

            _cnp.RefundTransactionReversal(reversal);
        }
        
        [Test]
        public void TestTransactionReversalWithLocation()
        {
            refundTransactionReversal reversal = new refundTransactionReversal();
            reversal.cnpTxnId = 3;
            reversal.amount = 2;
            reversal.surchargeAmount = 1;
            reversal.reportGroup = "Planets";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<amount>2</amount>\r\n<surchargeAmount>1</surchargeAmount>\r\n.*", RegexOptions.Singleline)))
                .Returns("<cnpOnlineResponse version='12.16' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><refundTransactionReversalResponse><cnpTxnId>123</cnpTxnId><location>sandbox</location></refundTransactionReversalResponse></cnpOnlineResponse>");

            var response = _cnp.RefundTransactionReversal(reversal);
            
            Assert.NotNull(response);
            Assert.AreEqual("sandbox", response.location);
        }

    }
}
