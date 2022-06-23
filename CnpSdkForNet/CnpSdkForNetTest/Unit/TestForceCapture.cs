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
    class TestForceCapture
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
        public void TestSecondaryAmount()
        {
            forceCapture capture = new forceCapture();
            capture.amount = 2;
            capture.secondaryAmount = 1;
            capture.orderSource = orderSourceType.ecommerce;
            capture.reportGroup = "Planets";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<amount>2</amount>\r\n<secondaryAmount>1</secondaryAmount>\r\n<orderSource>ecommerce</orderSource>.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><forceCaptureResponse><cnpTxnId>123</cnpTxnId></forceCaptureResponse></cnpOnlineResponse>");

            _cnp.ForceCapture(capture);
        }

        [Test]
        public void TestSurchargeAmount()
        {
            forceCapture capture = new forceCapture();
            capture.amount = 2;
            capture.surchargeAmount = 1;
            capture.orderSource = orderSourceType.ecommerce;
            capture.reportGroup = "Planets";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<amount>2</amount>\r\n<surchargeAmount>1</surchargeAmount>\r\n<orderSource>ecommerce</orderSource>.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><forceCaptureResponse><cnpTxnId>123</cnpTxnId></forceCaptureResponse></cnpOnlineResponse>");

            _cnp.ForceCapture(capture);
        }


        [Test]
        public void TestSurchargeAmount_Optional()
        {
            forceCapture capture = new forceCapture();
            capture.amount = 2;
            capture.orderSource = orderSourceType.ecommerce;
            capture.reportGroup = "Planets";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<amount>2</amount>\r\n<orderSource>ecommerce</orderSource>.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><forceCaptureResponse><cnpTxnId>123</cnpTxnId></forceCaptureResponse></cnpOnlineResponse>");

            _cnp.ForceCapture(capture);
        }

        [Test]
        public void TestDebtRepayment_True()
        {
            forceCapture forceCapture = new forceCapture();
            forceCapture.merchantData = new merchantDataType();
            forceCapture.debtRepayment = true;

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*</merchantData>\r\n<debtRepayment>true</debtRepayment>\r\n</forceCapture>.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.19' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><forceCaptureResponse><cnpTxnId>123</cnpTxnId></forceCaptureResponse></cnpOnlineResponse>");

            _cnp.ForceCapture(forceCapture);
        }

        [Test]
        public void TestDebtRepayment_False()
        {
            forceCapture forceCapture = new forceCapture();
            forceCapture.merchantData = new merchantDataType();
            forceCapture.debtRepayment = false;

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*</merchantData>\r\n<debtRepayment>false</debtRepayment>\r\n</forceCapture>.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.19' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><forceCaptureResponse><cnpTxnId>123</cnpTxnId></forceCaptureResponse></cnpOnlineResponse>");

            _cnp.ForceCapture(forceCapture);
        }

        [Test]
        public void TestDebtRepayment_Optional()
        {
            forceCapture forceCapture = new forceCapture();
            forceCapture.merchantData = new merchantDataType();

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*</merchantData>\r\n</forceCapture>.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.19' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><forceCaptureResponse><cnpTxnId>123</cnpTxnId></forceCaptureResponse></cnpOnlineResponse>");

            _cnp.ForceCapture(forceCapture);
        }

        [Test]
        public void TestProcessingType()
        {
            forceCapture capture = new forceCapture();
            capture.amount = 2;
            capture.orderSource = orderSourceType.ecommerce;
            capture.reportGroup = "Planets";
            capture.processingType = processingType.initialRecurring;
            
            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<amount>2</amount>\r\n<orderSource>ecommerce</orderSource>\r\n<processingType>initialRecurring</processingType>.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><forceCaptureResponse><cnpTxnId>123</cnpTxnId></forceCaptureResponse></cnpOnlineResponse>");

            _cnp.ForceCapture(capture);
        }

        [Test]
        public void TestUndefinedProcessingType()
        {
            forceCapture capture = new forceCapture();
            capture.amount = 2;
            capture.orderSource = orderSourceType.ecommerce;
            capture.reportGroup = "Planets";
            capture.processingType = processingType.undefined;

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<amount>2</amount>\r\n<orderSource>ecommerce</orderSource>.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><forceCaptureResponse><cnpTxnId>123</cnpTxnId></forceCaptureResponse></cnpOnlineResponse>");

            _cnp.ForceCapture(capture);
        }

        [Test]
        public void TestForceCaptureWithMCC()
        {
            forceCapture capture = new forceCapture();
            capture.amount = 2;
            capture.merchantCategoryCode = "0111";
            capture.orderSource = orderSourceType.ecommerce;
            capture.reportGroup = "Planets";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<amount>2</amount>\r\n<orderSource>ecommerce</orderSource>\r\n<merchantCategoryCode>0111</merchantCategoryCode>.*", RegexOptions.Singleline)))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><forceCaptureResponse><cnpTxnId>123</cnpTxnId></forceCaptureResponse></cnpOnlineResponse>");

            _cnp.ForceCapture(capture);
        }
        
        [Test]
        public void TestForceCaptureWithLocation()
        {
            forceCapture capture = new forceCapture();
            capture.amount = 2;
            capture.secondaryAmount = 1;
            capture.orderSource = orderSourceType.ecommerce;
            
            capture.reportGroup = "Planets";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<amount>2</amount>\r\n<secondaryAmount>1</secondaryAmount>\r\n<orderSource>ecommerce</orderSource>.*", RegexOptions.Singleline)))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><forceCaptureResponse><cnpTxnId>123</cnpTxnId><location>sandbox</location></forceCaptureResponse></cnpOnlineResponse>");

            var response = _cnp.ForceCapture(capture);
            
            Assert.NotNull(response);
            Assert.AreEqual("sandbox", response.location);
        }
    }
}
