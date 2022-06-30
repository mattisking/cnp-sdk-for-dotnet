﻿using System.Collections.Generic;
using NUnit.Framework;
using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Moq;
using Cnp.Sdk.Interfaces;
using Cnp.Sdk.Core;
using System.Net.Http;

namespace Cnp.Sdk.Test.Functional
{
    [TestFixture]
    internal class TestCaptureGivenAuth
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
        public void SimpleCaptureGivenAuthWithCard()
        {
            var capturegivenauth = new captureGivenAuth
            {
                id = "1",
                amount = 106,
                orderId = "12344",
                authInformation = new authInformation
                {
                    authDate = new DateTime(2002, 10, 9),
                    authCode = "543216",
                    authAmount = 12345,
                },
                orderSource = orderSourceType.ecommerce,
                card = new cardType
                {
                    type = methodOfPaymentTypeEnum.VI,
                    number = "4100000000000000",
                    expDate = "1210",
                },
                processingType = processingType.accountFunding,
                originalNetworkTransactionId = "abc123",
                originalTransactionAmount = 123456789
            };

            var response = _cnp.CaptureGivenAuth(capturegivenauth);
            Assert.AreEqual("Approved", response.message);
        }

        [Test]
        public void SimpleCaptureGivenAuthWithMpos()
        {
            var capturegivenauth = new captureGivenAuth
            {
                id = "1",
                amount = 500,
                orderId = "12344",
                authInformation = new authInformation
                {
                    authDate = new DateTime(2002, 10, 9),
                    authCode = "543216",
                    authAmount = 12345
                },
                orderSource = orderSourceType.ecommerce,
                mpos = new mposType
                {
                    ksn = "77853211300008E00016",
                    encryptedTrack = "CASE1E185EADD6AFE78C9A214B21313DCD836FDD555FBE3A6C48D141FE80AB9172B963265AFF72111895FE415DEDA162CE8CB7AC4D91EDB611A2AB756AA9CB1A000000000000000000000000000000005A7AAF5E8885A9DB88ECD2430C497003F2646619A2382FFF205767492306AC804E8E64E8EA6981DD",
                    formatId = "30",
                    track1Status = 0,
                    track2Status = 0
                }
            };

            var response = _cnp.CaptureGivenAuth(capturegivenauth);
            Assert.AreEqual("Approved", response.message);
        }

        [Test]
        public void SimpleCaptureGivenAuthWithToken()
        {
            var capturegivenauth = new captureGivenAuth
            {
                id = "1",
                amount = 106,
                orderId = "12344",
                authInformation = new authInformation
                {
                    authDate = new DateTime(2002, 10, 9),
                    authCode = "543216",
                    authAmount = 12345,
                },

                orderSource = orderSourceType.ecommerce,
                token = new cardTokenType
                {
                    cnpToken = "123456789101112",
                    expDate = "1210",
                    cardValidationNum = "555",
                    type = methodOfPaymentTypeEnum.VI
                }
            };

            var response = _cnp.CaptureGivenAuth(capturegivenauth);
            Assert.AreEqual("Approved", response.message);
        }

        [Test]
        public void ComplexCaptureGivenAuth()
        {
            var capturegivenauth = new captureGivenAuth
            {
                id = "1",
                amount = 106,
                orderId = "12344",
                authInformation = new authInformation
                {
                    authDate = new DateTime(2002, 10, 9),
                    authCode = "543216",
                    authAmount = 12345
                },
                billToAddress = new contact
                {
                    name = "Bob",
                    city = "lowell",
                    state = "MA",
                    email = "cnp.com"
                },

                processingInstructions = new processingInstructions
                {
                    bypassVelocityCheck = true
                },
                orderSource = orderSourceType.ecommerce,
                card = new cardType
                {
                    type = methodOfPaymentTypeEnum.VI,
                    number = "4100000000000000",
                    expDate = "1210"
                },
                processingType = processingType.initialInstallment,
                originalNetworkTransactionId = "abc123",
                originalTransactionAmount = 123456789
            };



            var response = _cnp.CaptureGivenAuth(capturegivenauth);
            Assert.AreEqual("Approved", response.message);
        }

        [Test]
        public void AuthInfo()
        {
            var capturegivenauth = new captureGivenAuth
            {
                id = "1",
                amount = 106,
                orderId = "12344",
                authInformation = new authInformation
                {
                    authDate = new DateTime(2002, 10, 9),
                    authCode = "543216",
                    authAmount = 12345,
                    fraudResult = new fraudResult
                    {
                        avsResult = "12",
                        cardValidationResult = "123",
                        authenticationResult = "1",
                        advancedAVSResult = "123"
                    }
                },
                orderSource = orderSourceType.ecommerce,
                card = new cardType
                {
                    type = methodOfPaymentTypeEnum.VI,
                    number = "4100000000000000",
                    expDate = "1210"
                },
                processingType = processingType.initialRecurring,
                originalNetworkTransactionId = "abc123",
                originalTransactionAmount = 123456789,
            };

            var response = _cnp.CaptureGivenAuth(capturegivenauth);
            Assert.AreEqual("Approved", response.message);
        }

        [Test]
        public void SimpleCaptureGivenAuthWithTokenAndSpecialCharacters()
        {
            var capturegivenauth = new captureGivenAuth
            {
                id = "1",
                amount = 106,
                orderId = "<'&\">",
                authInformation = new authInformation
                {
                    authDate = new DateTime(2002, 10, 9),
                    authCode = "543216",
                    authAmount = 12345
                },
                orderSource = orderSourceType.ecommerce,
                token = new cardTokenType
                {
                    cnpToken = "123456789101112",
                    expDate = "1210",
                    cardValidationNum = "555",
                    type = methodOfPaymentTypeEnum.VI
                }
            };

            var response = _cnp.CaptureGivenAuth(capturegivenauth);
            Assert.AreEqual("Approved", response.message);
        }

        [Test]
        public void SimpleCaptureGivenAuthWithSecondaryAmount()
        {
            var capturegivenauth = new captureGivenAuth
            {
                id = "1",
                amount = 106,
                secondaryAmount = 50,
                orderId = "12344",
                authInformation = new authInformation
                {
                    authDate = new DateTime(2002, 10, 9),
                    authCode = "543216",
                    authAmount = 12345
                },

                orderSource = orderSourceType.ecommerce,
                card = new cardType
                {
                    type = methodOfPaymentTypeEnum.VI,
                    number = "4100000000000000",
                    expDate = "1210",
                },

                processingType = processingType.accountFunding,
                originalNetworkTransactionId = "abc123",
                originalTransactionAmount = 123456789
            };

            var response = _cnp.CaptureGivenAuth(capturegivenauth);
            Assert.AreEqual("Approved", response.message);
        }

        [Test]
        public void TestCaptureGivenAuthAsync()
        {
            var capturegivenauth = new captureGivenAuth
            {
                id = "1",
                amount = 106,
                orderId = "12344",
                authInformation = new authInformation
                {
                    authDate = new DateTime(2002, 10, 9),
                    authCode = "543216",
                    authAmount = 12345,
                },
                orderSource = orderSourceType.ecommerce,
                card = new cardType
                {
                    type = methodOfPaymentTypeEnum.VI,
                    number = "4100000000000000",
                    expDate = "1210",
                },
                processingType = processingType.accountFunding,
                originalNetworkTransactionId = "abc123",
                originalTransactionAmount = 123456789
            };
            CancellationToken cancellationToken = new CancellationToken(false);
            var response = _cnp.CaptureGivenAuthAsync(capturegivenauth, cancellationToken);
            Assert.AreEqual("000", response.Result.response);
        }
        
        [Test]
        public void SimpleCaptureGivenAuthWithCardWithLocation()
        {
            var capturegivenauth = new captureGivenAuth
            {
                id = "1",
                amount = 106,
                orderId = "12344",
                authInformation = new authInformation
                {
                    authDate = new DateTime(2002, 10, 9),
                    authCode = "543216",
                    authAmount = 12345,
                },
                orderSource = orderSourceType.ecommerce,
                card = new cardType
                {
                    type = methodOfPaymentTypeEnum.VI,
                    number = "4100000000000000",
                    expDate = "1210",
                },
                processingType = processingType.accountFunding,
                originalNetworkTransactionId = "abc123",
                originalTransactionAmount = 123456789
            };

            var response = _cnp.CaptureGivenAuth(capturegivenauth);
            Assert.AreEqual("sandbox", response.location);
            Assert.AreEqual("Approved", response.message);
        }
        
        [Test]
        public void SimpleCaptureGivenAuthWithBusinessIndicator()
        {
            var capturegivenauth = new captureGivenAuth
            {
                id = "1",
                amount = 106,
                orderId = "12344",
                authInformation = new authInformation
                {
                    authDate = new DateTime(2002, 10, 9),
                    authCode = "543216",
                    authAmount = 12345,
                },
                orderSource = orderSourceType.ecommerce,
                card = new cardType
                {
                    type = methodOfPaymentTypeEnum.VI,
                    number = "4100000000000000",
                    expDate = "1210",
                },
                processingType = processingType.accountFunding,
                businessIndicator = businessIndicatorEnum.consumerBillPayment,
                originalNetworkTransactionId = "abc123",
                originalTransactionAmount = 123456789
            };

            var response = _cnp.CaptureGivenAuth(capturegivenauth);
            Assert.AreEqual("Approved", response.message);
        }

        [Test]
        public void SimpleCaptureGivenAuthWithRetailerAddressAndAdditionalCOFdata()///12.24
        {
            
            var capturegivenauth = new captureGivenAuth
            {
                id = "1",
                amount = 106,
                orderId = "12344",
                crypto = false,
                authInformation = new authInformation
                {
                    authDate = new DateTime(2002, 10, 9),
                    authCode = "543216",
                    authAmount = 12345,
                },
                orderSource = orderSourceType.ecommerce,
                card = new cardType
                {
                    type = methodOfPaymentTypeEnum.VI,
                    number = "4100000000000000",
                    expDate = "1210",
                },
                retailerAddress = new contact
                {
                    name = "Mikasa Ackerman",
                    addressLine1 = "1st Main Street",
                    city = "Burlington",
                    state = "MA",
                    country = countryTypeEnum.USA,
                    email = "mikasa@cnp.com",
                    zip = "01867-4456",
                    sellerId = "s1234",
                    url = "www.google.com"
                },
                additionalCOFData = new additionalCOFData()
                {
                    totalPaymentCount = "35",
                    paymentType = paymentTypeEnum.Fixed_Amount,
                    uniqueId = "12345wereew233",
                    frequencyOfMIT = frequencyOfMITEnum.BiWeekly,
                    validationReference = "re3298rhriw4wrw",
                    sequenceIndicator = 2
                },
                processingType = processingType.accountFunding,
                originalNetworkTransactionId = "abc123",
                originalTransactionAmount = 123456789
            };

            var response = _cnp.CaptureGivenAuth(capturegivenauth);
            Assert.AreEqual("Approved", response.message);
        }
    }
}
