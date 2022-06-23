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
    class TestDepositTransactionReversal
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
            depositTransactionReversal reversal = new depositTransactionReversal();
            reversal.cnpTxnId = 3;
            reversal.amount = 2;
            reversal.surchargeAmount = 1;
            reversal.reportGroup = "Planets";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<amount>2</amount>\r\n<surchargeAmount>1</surchargeAmount>\r\n.*", RegexOptions.Singleline)))
                .Returns("<cnpOnlineResponse version='12.16' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><depositTransactionReversalResponse><cnpTxnId>3</cnpTxnId></depositTransactionReversalResponse></cnpOnlineResponse>");

            _cnp.DepositTransactionReversal(reversal);
        }

        [Test]
        public void TestSurchargeAmount_Optional()
        {
            depositTransactionReversal reversal = new depositTransactionReversal();
            reversal.cnpTxnId = 3;
            reversal.amount = 2;
            reversal.reportGroup = "Planets";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<amount>2</amount>\r\n.*", RegexOptions.Singleline)))
                .Returns("<cnpOnlineResponse version='12.16' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><depositTransactionReversalResponse><cnpTxnId>123</cnpTxnId></depositTransactionReversalResponse></cnpOnlineResponse>");

            _cnp.DepositTransactionReversal(reversal);
        }
        
        [Test]
        public void TestTransactionReversalWithLocation()
        {
            depositTransactionReversal reversal = new depositTransactionReversal();
            reversal.cnpTxnId = 3;
            reversal.amount = 2;
            reversal.surchargeAmount = 1;
            reversal.reportGroup = "Planets";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<amount>2</amount>\r\n<surchargeAmount>1</surchargeAmount>\r\n.*", RegexOptions.Singleline)))
                .Returns("<cnpOnlineResponse version='12.16' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><depositTransactionReversalResponse><cnpTxnId>123</cnpTxnId><location>sandbox</location></depositTransactionReversalResponse></cnpOnlineResponse>");

            var response = _cnp.DepositTransactionReversal(reversal);
            
            Assert.NotNull(response);
            Assert.AreEqual("sandbox", response.location);
        }

    }
}
