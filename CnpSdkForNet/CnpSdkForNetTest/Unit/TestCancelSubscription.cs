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
    class TestCancelSubscription
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
        public void TestSimple()
        {
            cancelSubscription update = new cancelSubscription();
            update.subscriptionId = 12345;
           
            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<cnpOnlineRequest.*?<cancelSubscription>\r\n<subscriptionId>12345</subscriptionId>\r\n</cancelSubscription>\r\n</cnpOnlineRequest>.*?.*", RegexOptions.Singleline)  ))
                .Returns("<cnpOnlineResponse version='8.20' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><cancelSubscriptionResponse><subscriptionId>12345</subscriptionId></cancelSubscriptionResponse></cnpOnlineResponse>");
     
            _cnp.CancelSubscription(update);
        }
        
        [Test]
        public void TestCancelSubscriptionWithLocation()
        {
            cancelSubscription update = new cancelSubscription();
            update.subscriptionId = 12345;
           
            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<cnpOnlineRequest.*?<cancelSubscription>\r\n<subscriptionId>12345</subscriptionId>\r\n</cancelSubscription>\r\n</cnpOnlineRequest>.*?.*", RegexOptions.Singleline)))
                .Returns("<cnpOnlineResponse version='8.20' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><cancelSubscriptionResponse><subscriptionId>12345</subscriptionId></cancelSubscriptionResponse></cnpOnlineResponse>");
     
            var response = _cnp.CancelSubscription(update);
            
            Assert.NotNull(response);
        }
    }
}
