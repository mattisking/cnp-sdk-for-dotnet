using System.Collections.Generic;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using Moq;
using Cnp.Sdk.Interfaces;
using Cnp.Sdk.Core;
using System.Net.Http;

namespace Cnp.Sdk.Test.Functional
{
    [TestFixture]
    internal class TestFastAccessFunding
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
        public void TestFastAccessFunding_token()
        {
            fastAccessFunding fastAccessFunding = new fastAccessFunding();
            fastAccessFunding.id = "A123456";
            fastAccessFunding.reportGroup = "FastPayment";
            fastAccessFunding.fundingSubmerchantId = "SomeSubMerchant";
            fastAccessFunding.submerchantName = "Some Merchant Inc.";
            fastAccessFunding.fundsTransferId = "123e4567e89b12d3";
            fastAccessFunding.amount = 3000;
            fastAccessFunding.token = new cardTokenType
            {
                cnpToken = "1111000101039449",
                expDate = "1112",
                cardValidationNum = "987",
                type = methodOfPaymentTypeEnum.VI,
            };
            
            var response = _cnp.FastAccessFunding(fastAccessFunding);
            Assert.AreEqual("000", response.response);
            Assert.AreEqual("sandbox", response.location);
            StringAssert.AreEqualIgnoringCase("Approved", response.message);
        }

        [Test]
        public void TestFastAccessFunding_tokenWithCardHolderAddress()
        {
            fastAccessFunding fastAccessFunding = new fastAccessFunding();
            fastAccessFunding.id = "A123456";
            fastAccessFunding.reportGroup = "FastPayment";
            fastAccessFunding.fundingSubmerchantId = "SomeSubMerchant";
            fastAccessFunding.submerchantName = "Some Merchant Inc.";
            fastAccessFunding.fundsTransferId = "123e4567e89b12d3";
            fastAccessFunding.amount = 3000;
            fastAccessFunding.token = new cardTokenType
            {
                cnpToken = "1111000101039449",
                expDate = "1112",
                cardValidationNum = "987",
                type = methodOfPaymentTypeEnum.VI,
            };

            addressType cardHolderAddressType = new addressType();
            cardHolderAddressType.addressLine1 = "37 Main Street";
            cardHolderAddressType.addressLine2 = "";
            cardHolderAddressType.addressLine3 = "";
            cardHolderAddressType.city = "Augusta";
            cardHolderAddressType.state = "Wisconsin";
            cardHolderAddressType.zip = "28209";
            cardHolderAddressType.country = countryTypeEnum.USA;

            fastAccessFunding.cardholderAddress = cardHolderAddressType;

            var response = _cnp.FastAccessFunding(fastAccessFunding);
            Assert.AreEqual("000", response.response);
            Assert.AreEqual("sandbox", response.location);
            StringAssert.AreEqualIgnoringCase("Approved", response.message);
        }

        [Test]
        [Ignore("Sandbox does not check for mismatch. Production does check.")]
        public void TestFastAccessFunding_mixedNames()
        {
            fastAccessFunding fastAccessFunding = new fastAccessFunding();
            fastAccessFunding.id = "A123456";
            fastAccessFunding.reportGroup = "FastPayment";
            fastAccessFunding.fundingSubmerchantId = "SomeSubMerchant";
            fastAccessFunding.customerName = "Some Customer";
            fastAccessFunding.fundsTransferId = "123e4567e89b12d3";
            fastAccessFunding.amount = 3000;
            fastAccessFunding.token = new cardTokenType
            {
                cnpToken = "1111000101039449",
                expDate = "1112",
                cardValidationNum = "987",
                type = methodOfPaymentTypeEnum.VI,
            };
            
            Assert.Throws<CnpOnlineException>(() => { _cnp.FastAccessFunding(fastAccessFunding); });
        }
    }
}