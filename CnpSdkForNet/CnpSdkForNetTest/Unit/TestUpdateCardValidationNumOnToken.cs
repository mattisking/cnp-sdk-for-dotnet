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
    class TestUpdateCardValidationNumOnToken
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
            updateCardValidationNumOnToken update = new updateCardValidationNumOnToken();
            update.orderId = "12344";
            update.cnpToken = "1111222233334444";
            update.cardValidationNum = "321";
            update.id = "123";
            update.reportGroup = "Default Report Group";
           
            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<updateCardValidationNumOnToken id=\"123\" reportGroup=\"Default Report Group\".*<orderId>12344</orderId>.*<cnpToken>1111222233334444</cnpToken>.*<cardValidationNum>321</cardValidationNum>.*</updateCardValidationNumOnToken>.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><updateCardValidationNumOnTokenResponse><cnpTxnId>4</cnpTxnId><orderId>12344</orderId><response>801</response><message>Token Successfully Registered</message><responseTime>2012-10-10T10:17:03</responseTime><location>sandbox</location></updateCardValidationNumOnTokenResponse></cnpOnlineResponse>");

            var response = _cnp.UpdateCardValidationNumOnToken(update);
            
            Assert.NotNull(response);
            Assert.AreEqual("sandbox", response.location);
        }

        [Test]
        public void TestOrderIdIsOptional()
        {
            updateCardValidationNumOnToken update = new updateCardValidationNumOnToken();
            update.orderId = null;
            update.cnpToken = "1111222233334444";
            update.cardValidationNum = "321";
            update.id = "123";
            update.reportGroup = "Default Report Group";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<updateCardValidationNumOnToken id=\"123\" reportGroup=\"Default Report Group\".*<cnpToken>1111222233334444</cnpToken>.*<cardValidationNum>321</cardValidationNum>.*</updateCardValidationNumOnToken>.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><updateCardValidationNumOnTokenResponse><cnpTxnId>4</cnpTxnId><response>801</response><message>Token Successfully Registered</message><responseTime>2012-10-10T10:17:03</responseTime></updateCardValidationNumOnTokenResponse></cnpOnlineResponse>");

            updateCardValidationNumOnTokenResponse response = _cnp.UpdateCardValidationNumOnToken(update);
            Assert.IsNotNull(response);

        }

    }
}
