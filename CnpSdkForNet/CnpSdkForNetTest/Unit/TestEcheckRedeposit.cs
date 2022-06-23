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
    class TestEcheckRedeposit
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
        public void TestMerchantData()
        {
            echeckRedeposit echeckRedeposit = new echeckRedeposit();
            echeckRedeposit.cnpTxnId = 1;
            echeckRedeposit.merchantData = new merchantDataType();
            echeckRedeposit.merchantData.campaign = "camp";
            echeckRedeposit.merchantData.affiliate = "affil";
            echeckRedeposit.merchantData.merchantGroupingId = "mgi";
            echeckRedeposit.customIdentifier = "customIdent";
           
            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<echeckRedeposit.*<cnpTxnId>1</cnpTxnId>.*<merchantData>.*<campaign>camp</campaign>.*<affiliate>affil</affiliate>.*<merchantGroupingId>mgi</merchantGroupingId>.*</merchantData>.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.13' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><echeckRedepositResponse><cnpTxnId>123</cnpTxnId></echeckRedepositResponse></cnpOnlineResponse>");
     
            _cnp.EcheckRedeposit(echeckRedeposit);
        }     
        
        [Test]
        public void TestEcheckRedepositWithLocation()
        {
            echeckRedeposit echeckRedeposit = new echeckRedeposit();
            echeckRedeposit.cnpTxnId = 1;
            echeckRedeposit.merchantData = new merchantDataType();
            echeckRedeposit.merchantData.campaign = "camp";
            echeckRedeposit.merchantData.affiliate = "affil";
            echeckRedeposit.merchantData.merchantGroupingId = "mgi";
            echeckRedeposit.customIdentifier = "customIdent";
           
            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<echeckRedeposit.*<cnpTxnId>1</cnpTxnId>.*<merchantData>.*<campaign>camp</campaign>.*<affiliate>affil</affiliate>.*<merchantGroupingId>mgi</merchantGroupingId>.*</merchantData>.*", RegexOptions.Singleline)))
                .Returns("<cnpOnlineResponse version='8.13' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><echeckRedepositResponse><cnpTxnId>123</cnpTxnId><location>sandbox</location></echeckRedepositResponse></cnpOnlineResponse>");
     
            var response = _cnp.EcheckRedeposit(echeckRedeposit);
            
            Assert.NotNull(response);
            Assert.AreEqual("sandbox", response.location);
        }            
    }
}
