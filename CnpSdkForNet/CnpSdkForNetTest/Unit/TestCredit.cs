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
    class TestCredit
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
        public void TestActionReasonOnOrphanedRefund()
        {
            credit credit = new credit();
            credit.orderId = "12344";
            credit.amount = 2;
            credit.orderSource = orderSourceType.ecommerce;
            credit.reportGroup = "Planets";
            credit.actionReason = "SUSPECT_FRAUD";
           
            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<actionReason>SUSPECT_FRAUD</actionReason>.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.10' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><creditResponse><cnpTxnId>123</cnpTxnId></creditResponse></cnpOnlineResponse>");
     
            _cnp.Credit(credit);
        }

        [Test]
        public void TestOrderSource_Set()
        {
            credit credit = new credit();
            credit.orderId = "12344";
            credit.amount = 2;
            credit.orderSource = orderSourceType.ecommerce;
            credit.reportGroup = "Planets";
            // credit.pin = "1234";
            // .*<pin>1234</pin>

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<credit.*<amount>2</amount>.*<orderSource>ecommerce</orderSource>.*</credit>.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.10' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><creditResponse><cnpTxnId>123</cnpTxnId></creditResponse></cnpOnlineResponse>");

            _cnp.Credit(credit);
        }

        [Test]
        public void TestSecondaryAmount_Orphan()
        {
            credit credit = new credit();
            credit.amount = 2;
            credit.secondaryAmount = 1;
            credit.orderId = "3";
            credit.orderSource = orderSourceType.ecommerce;
            credit.reportGroup = "Planets";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<orderId>3</orderId>\r\n<amount>2</amount>\r\n<secondaryAmount>1</secondaryAmount>\r\n<orderSource>ecommerce</orderSource>.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><creditResponse><cnpTxnId>123</cnpTxnId></creditResponse></cnpOnlineResponse>");

            _cnp.Credit(credit);
        }

        [Test]
        public void TestSecondaryAmount_Tied()
        {
            credit credit = new credit();
            credit.amount = 2;
            credit.secondaryAmount = 1;
            credit.cnpTxnId = 3;
            credit.processingInstructions = new processingInstructions();
            credit.reportGroup = "Planets";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<cnpTxnId>3</cnpTxnId>\r\n<amount>2</amount>\r\n<secondaryAmount>1</secondaryAmount>\r\n<process.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><creditResponse><cnpTxnId>123</cnpTxnId></creditResponse></cnpOnlineResponse>");

            _cnp.Credit(credit);
        }

        [Test]
        public void TestSurchargeAmount_Tied()
        {
            credit credit = new credit();
            credit.amount = 2;
            credit.surchargeAmount = 1;
            credit.cnpTxnId = 3;
            credit.processingInstructions = new processingInstructions();
            credit.reportGroup = "Planets";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<cnpTxnId>3</cnpTxnId>\r\n<amount>2</amount>\r\n<surchargeAmount>1</surchargeAmount>\r\n<process.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><creditResponse><cnpTxnId>123</cnpTxnId></creditResponse></cnpOnlineResponse>");

            _cnp.Credit(credit);
        }

        [Test]
        public void TestSurchargeAmount_TiedOptional()
        {
            credit credit = new credit();
            credit.amount = 2;
            credit.cnpTxnId = 3;
            credit.reportGroup = "Planets";
            credit.processingInstructions = new processingInstructions();

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<cnpTxnId>3</cnpTxnId>\r\n<amount>2</amount>\r\n<processi.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><creditResponse><cnpTxnId>123</cnpTxnId></creditResponse></cnpOnlineResponse>");

            _cnp.Credit(credit);
        }

        [Test]
        public void TestSurchargeAmount_Orphan()
        {
            credit credit = new credit();
            credit.amount = 2;
            credit.surchargeAmount = 1;
            credit.orderId = "3";
            credit.orderSource = orderSourceType.ecommerce;
            credit.reportGroup = "Planets";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<orderId>3</orderId>\r\n<amount>2</amount>\r\n<surchargeAmount>1</surchargeAmount>\r\n<orderSource>ecommerce</orderSource>.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><creditResponse><cnpTxnId>123</cnpTxnId></creditResponse></cnpOnlineResponse>");

            _cnp.Credit(credit);
        }

        [Test]
        public void TestSurchargeAmount_OrphanOptional()
        {
            credit credit = new credit();
            credit.amount = 2;
            credit.orderId = "3";
            credit.orderSource = orderSourceType.ecommerce;
            credit.reportGroup = "Planets";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<orderId>3</orderId>\r\n<amount>2</amount>\r\n<orderSource>ecommerce</orderSource>.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><creditResponse><cnpTxnId>123</cnpTxnId></creditResponse></cnpOnlineResponse>");

            _cnp.Credit(credit);
        }

        [Test]
        public void TestPos_Tied()
        {
            credit credit = new credit();
            credit.amount = 2;
            credit.pos = new pos();
            credit.pos.terminalId = "abc123";
            credit.cnpTxnId = 3;
            credit.reportGroup = "Planets";
            credit.payPalNotes = "notes";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<cnpTxnId>3</cnpTxnId>\r\n<amount>2</amount>\r\n<pos>\r\n<terminalId>abc123</terminalId></pos>\r\n<payPalNotes>.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><creditResponse><cnpTxnId>123</cnpTxnId></creditResponse></cnpOnlineResponse>");

            _cnp.Credit(credit);
        }

        [Test]
        public void TestPos_TiedOptional()
        {
            credit credit = new credit();
            credit.amount = 2;
            credit.cnpTxnId = 3;
            credit.reportGroup = "Planets";
            credit.payPalNotes = "notes";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<cnpTxnId>3</cnpTxnId>\r\n<amount>2</amount>\r\n<payPalNotes>.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><creditResponse><cnpTxnId>123</cnpTxnId></creditResponse></cnpOnlineResponse>");

           _cnp.Credit(credit);
        }

        [Test]
        public void TestCreditWithPin()
        {
            credit credit = new credit();
            credit.id = "1";
            credit.reportGroup = "planets";
            credit.cnpTxnId = 123456000;
            credit.pin = "1234";
            cardType card = new cardType();
            card.type = methodOfPaymentTypeEnum.VI;
            card.number = "4100000000000001";
            card.expDate = "1210";
            credit.card = card;

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<credit.*<pin>1234</pin>.*</credit>.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.10' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><creditResponse><cnpTxnId>123</cnpTxnId></creditResponse></cnpOnlineResponse>");

            _cnp.Credit(credit);
        }

        [Test]
        public void TestCreditWithMCC()
        {
            credit credit = new credit();
            credit.amount = 2;
            credit.merchantCategoryCode = "0111";
            credit.orderId = "3";
            credit.orderSource = orderSourceType.ecommerce;
            credit.reportGroup = "Planets";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<orderId>3</orderId>\r\n<amount>2</amount>\r\n<orderSource>ecommerce</orderSource>\r\n<merchantCategoryCode>0111</merchantCategoryCode>.*", RegexOptions.Singleline)))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><creditResponse><cnpTxnId>123</cnpTxnId></creditResponse></cnpOnlineResponse>");

            _cnp.Credit(credit);
        }
        
        [Test]
        public void TestCreditWithLocation()
        {
            credit credit = new credit();
            credit.orderId = "12344";
            credit.amount = 2;
            credit.orderSource = orderSourceType.ecommerce;
            credit.reportGroup = "Planets";
            credit.actionReason = "SUSPECT_FRAUD";
           
            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<actionReason>SUSPECT_FRAUD</actionReason>.*", RegexOptions.Singleline)))
                .Returns("<cnpOnlineResponse version='8.10' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><creditResponse><cnpTxnId>123</cnpTxnId><location>sandbox</location></creditResponse></cnpOnlineResponse>");
     
            var response = _cnp.Credit(credit);
            
            Assert.NotNull(response);
            Assert.AreEqual("sandbox", response.location);
        }
    }
}
