using System.Collections.Generic;
using NUnit.Framework;
using System.Threading;
using Microsoft.Extensions.Logging;
using Moq;
using Cnp.Sdk.Interfaces;
using Cnp.Sdk.Core;
using System.Net.Http;

namespace Cnp.Sdk.Test.Functional
{
    [TestFixture]
    internal class TestBalanceInquiry
    {
        private CnpOnline _cnp;
        private Mock<ILogger<CnpOnline>> _mockLogger;
        private Mock<ILogger<Communications>> _mockComLogger;
        private ICommunications _communications;

        [OneTimeSetUp]
        public void SetUpCnp()
        {
            _mockLogger = new Mock<ILogger<CnpOnline>>();
            _mockComLogger = new Mock<ILogger<Communications>>();

            var configManager = new ConfigManager();
            var config = configManager.getConfig();
            var handler = new CommunicationsHttpClientHandler(config);
            _communications = new Communications(new HttpClient(handler), _mockComLogger.Object, config);

            _cnp = new CnpOnline(_communications, config, _mockLogger.Object);
        }

        [Test]
        public void SimpleBalanceInquiry()
        {
            var balanceInquiry = new balanceInquiry
            {
                id = "1",
                reportGroup = "Planets",
                orderId = "12344",
                orderSource = orderSourceType.ecommerce,
                card = new giftCardCardType
                {
                    type = methodOfPaymentTypeEnum.GC,
                    number = "414100000000000000",
                    cardValidationNum = "123",
                    expDate = "1215",
                }
            };
        
        var response = _cnp.BalanceInquiry(balanceInquiry);
        Assert.AreEqual("000", response.response);
        Assert.AreEqual("sandbox", response.location);
        }

        [Test]
        public void TestBalanceInquiryAsync()
        {
            var balanceInquiry = new balanceInquiry
            {
                id = "1",
                reportGroup = "Planets",
                orderId = "12344",
                orderSource = orderSourceType.ecommerce,
                card = new giftCardCardType
                {
                    type = methodOfPaymentTypeEnum.GC,
                    number = "414100000000000000",
                    cardValidationNum = "123",
                    expDate = "1215",
                }
            };

            CancellationToken cancellationToken = new CancellationToken(false);
            var response = _cnp.BalanceInquiryAsync(balanceInquiry, cancellationToken);
            Assert.AreEqual("000", response.Result.response);
        }

        [Test]
        public void TestBalanceInquiryAsync_newMerchantId()
        {
            _cnp.SetMerchantId("1234");
            var balanceInquiry = new balanceInquiry
            {
                id = "1",
                reportGroup = "Planets",
                orderId = "12344",
                orderSource = orderSourceType.ecommerce,
                card = new giftCardCardType
                {
                    type = methodOfPaymentTypeEnum.GC,
                    number = "414100000000000000",
                    cardValidationNum = "123",
                    expDate = "1215",
                }
            };

            CancellationToken cancellationToken = new CancellationToken(false);
            var response = _cnp.BalanceInquiryAsync(balanceInquiry, cancellationToken);
            Assert.AreEqual("000", response.Result.response);
        }

    }
}
