﻿using NUnit.Framework;
using Moq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Cnp.Sdk.Interfaces;

namespace Cnp.Sdk.Test.Unit
{
    [TestFixture]
    class TestRegisterTokenRequest
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
        public void TestSimpleRequest()
        {
            registerTokenRequestType register = new registerTokenRequestType();
            register.orderId = "12344";
            register.accountNumber = "4100000000000001";
           
            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<registerTokenRequest.*<accountNumber>4100000000000001</accountNumber>.*</registerTokenRequest>.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><registerTokenResponse><cnpTxnId>4</cnpTxnId><response>801</response><message>Token Successfully Registered</message><responseTime>2012-10-10T10:17:03</responseTime><location>sandbox</location></registerTokenResponse></cnpOnlineResponse>");

            var response = _cnp.RegisterToken(register);
            
            Assert.NotNull(response);
            Assert.AreEqual("sandbox", response.location);
        }

        [Test]
        public void TestCanContainCardValidationNum()
        {
            registerTokenRequestType register = new registerTokenRequestType();
            register.orderId = "12344";
            register.accountNumber = "4100000000000001";
            register.cardValidationNum = "123";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<registerTokenRequest.*<accountNumber>4100000000000001</accountNumber>.*<cardValidationNum>123</cardValidationNum>.*</registerTokenRequest>.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><registerTokenResponse><cnpTxnId>4</cnpTxnId><response>801</response><message>Token Successfully Registered</message><responseTime>2012-10-10T10:17:03</responseTime></registerTokenResponse></cnpOnlineResponse>");

            _cnp.RegisterToken(register);
        }

        [Test]
        public void TestSimpleRequestWithApplepay()
        {
            registerTokenRequestType register = new registerTokenRequestType();
            register.orderId = "12344";
            applepayType applepay = new applepayType();
            applepayHeaderType applepayHeaderType = new applepayHeaderType();
            applepayHeaderType.applicationData = "454657413164";
            applepayHeaderType.ephemeralPublicKey = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";
            applepayHeaderType.publicKeyHash = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";
            applepayHeaderType.transactionId = "1234";
            applepay.header = applepayHeaderType;
            applepay.data = "user";
            applepay.signature = "sign";
            applepay.version = "1";
            register.applepay = applepay;

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<registerTokenRequest.*<applepay>.*?<data>user</data>.*?</applepay>.*?</registerTokenRequest>.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><registerTokenResponse><cnpTxnId>4</cnpTxnId><response>801</response><message>Token Successfully Registered</message><responseTime>2012-10-10T10:17:03</responseTime></registerTokenResponse></cnpOnlineResponse>");

            _cnp.RegisterToken(register);
        }

    }
}
