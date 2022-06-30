﻿using System.Collections.Generic;
using System.Net.Http;
using Cnp.Sdk.Configuration;
using Cnp.Sdk.Interfaces;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Cnp.Sdk.Test.Certification
{
    [TestFixture]
    class TestCert3AuthReversal
    {
        private CnpOnline cnp;
        private ILogger<CnpOnline> _logger;
        private ILogger<Communications> _comlogger;
        private ICommunications _communications;

        [OneTimeSetUp]
        public void SetUp()
        {
            EnvironmentVariableTestFlags.RequirePreliveOnlineTestsEnabled();
            
            var existingConfig = new ConfigManager().getConfig();
            CnpOnlineConfig config = new CnpOnlineConfig();
            config.Url = "https://payments.vantivprelive.com/vap/communicator/online";
            config.ReportGroup = "Default Report Group";
            config.Username = existingConfig.Username;
            config.Timeout = 20000;
            config.MerchantId = existingConfig.MerchantId;
            config.Password = existingConfig.Password;
            config.NeuterAccountNums = false;
            config.ProxyHost = string.Empty;
            config.ProxyPort = 0;
            ConfigManager configManager = new ConfigManager(config);
            _communications = new Communications(new HttpClient(), _comlogger, configManager.getConfig());

            _logger = LoggerFactory.Create(config =>
            {
                config.AddConsole();
            }).CreateLogger<CnpOnline>();

            cnp = new CnpOnline(_communications, configManager.getConfig(), _logger);
        }

        //[OneTimeTearDown]
        //public void Dispose()
        //{
        //    Communications.DisposeHttpClient();
        //}

        [Test]
        public void Test32()
        {
            authorization auth = new authorization();
            auth.id = "1";
            auth.orderId = "32";
            auth.amount = 10010;
            auth.orderSource = orderSourceType.ecommerce;
            contact billToAddress = new contact();
            billToAddress.name = "John Smith";
            billToAddress.addressLine1 = "1 Main St.";
            billToAddress.city = "Burlington";
            billToAddress.state = "MA";
            billToAddress.zip = "01803-3747";
            billToAddress.country = countryTypeEnum.US;
            auth.billToAddress = billToAddress;
            cardType card = new cardType();
            card.number = "4457010000000009";
            card.expDate = "0112";
            card.cardValidationNum = "349";
            card.type = methodOfPaymentTypeEnum.VI;
            auth.card = card;

            authorizationResponse authorizeResponse = cnp.Authorize(auth);
//            Assert.AreEqual("111", authorizeResponse.response);
//            Assert.AreEqual("Authorization amount has already been depleted", authorizeResponse.message);
            Assert.AreEqual("11111 ", authorizeResponse.authCode);
            Assert.AreEqual("01", authorizeResponse.fraudResult.avsResult);
            Assert.AreEqual("M", authorizeResponse.fraudResult.cardValidationResult);

            capture capture = new capture();
            capture.id = authorizeResponse.id;
            capture.cnpTxnId = authorizeResponse.cnpTxnId;
            capture.amount = 5005;
            captureResponse captureResponse = cnp.Capture(capture);
            Assert.AreEqual("000", captureResponse.response);
            Assert.AreEqual("Approved", captureResponse.message);

            authReversal reversal = new authReversal();
            reversal.id = authorizeResponse.id;
            reversal.cnpTxnId = 320000000000000000;
            authReversalResponse reversalResponse = cnp.AuthReversal(reversal);
            Assert.AreEqual("000", reversalResponse.response);
            Assert.AreEqual("Approved", reversalResponse.message);
        }

        [Test]
        public void Test33()
        {
            authorization auth = new authorization();
            auth.id = "1";
            auth.orderId = "33";
            auth.amount = 20020;
            auth.orderSource = orderSourceType.ecommerce;
            contact billToAddress = new contact();
            billToAddress.name = "Mike J. Hammer";
            billToAddress.addressLine1 = "2 Main St.";
            billToAddress.addressLine2 = "Apt. 222";
            billToAddress.city = "Riverside";
            billToAddress.state = "RI";
            billToAddress.zip = "02915";
            billToAddress.country = countryTypeEnum.US;
            auth.billToAddress = billToAddress;
            cardType card = new cardType();
            card.number = "5112010000000003";
            card.expDate = "0212";
            card.cardValidationNum = "261";
            card.type = methodOfPaymentTypeEnum.MC;
            auth.card = card;
            fraudCheckType fraud = new fraudCheckType();
            fraud.authenticationValue = "BwABBJQ1AgAAAAAgJDUCAAAAAAA=";
            auth.cardholderAuthentication = fraud;

            authorizationResponse authorizeResponse = cnp.Authorize(auth);
            Assert.AreEqual("000", authorizeResponse.response);
            Assert.AreEqual("Approved", authorizeResponse.message);
            Assert.AreEqual("22222", authorizeResponse.authCode.Trim());
            Assert.AreEqual("10", authorizeResponse.fraudResult.avsResult);
            Assert.AreEqual("M", authorizeResponse.fraudResult.cardValidationResult);

            authReversal reversal = new authReversal();
            reversal.id = authorizeResponse.id;
            reversal.cnpTxnId = authorizeResponse.cnpTxnId;
            authReversalResponse reversalResponse = cnp.AuthReversal(reversal);
            Assert.AreEqual("000", reversalResponse.response);
            Assert.AreEqual("Approved", reversalResponse.message);
        }

        [Test]
        public void Test34()
        {
            authorization auth = new authorization();
            auth.id = "1";
            //auth.cnpTxnId = 12345678000L;
            auth.orderId = "34";
            auth.amount = 30030;
            auth.orderSource = orderSourceType.ecommerce;
            contact billToAddress = new contact();
            billToAddress.name = "Eileen Jones";
            billToAddress.addressLine1 = "3 Main St.";
            billToAddress.city = "Bloomfield";
            billToAddress.state = "CT";
            billToAddress.zip = "06002";
            billToAddress.country = countryTypeEnum.US;
            auth.billToAddress = billToAddress;
            cardType card = new cardType();
            card.number = "6011010000000003";
            card.expDate = "0312";
            card.cardValidationNum = "758";
            card.type = methodOfPaymentTypeEnum.DI;
            auth.card = card;

            authorizationResponse authorizeResponse = cnp.Authorize(auth);
            Assert.AreEqual("000", authorizeResponse.response);
            Assert.AreEqual("Approved", authorizeResponse.message);
            Assert.AreEqual("33333", authorizeResponse.authCode.Trim());
            Assert.AreEqual("10", authorizeResponse.fraudResult.avsResult);
            Assert.AreEqual("M", authorizeResponse.fraudResult.cardValidationResult);

            authReversal reversal = new authReversal();
            reversal.id = authorizeResponse.id;
            reversal.cnpTxnId = authorizeResponse.cnpTxnId;
            authReversalResponse reversalResponse = cnp.AuthReversal(reversal);
            Assert.AreEqual("000", reversalResponse.response);
            Assert.AreEqual("Approved", reversalResponse.message);
        }

        [Test]
        public void Test35()
        {
            authorization auth = new authorization();
            auth.id = "1";
            auth.orderId = "35";
            auth.amount = 10100;
            auth.orderSource = orderSourceType.ecommerce;
            contact billToAddress = new contact();
            billToAddress.name = "Bob Black";
            billToAddress.addressLine1 = "4 Main St.";
            billToAddress.city = "Laurel";
            billToAddress.state = "MD";
            billToAddress.zip = "20708";
            billToAddress.country = countryTypeEnum.US;
            auth.billToAddress = billToAddress;
            cardType card = new cardType();
            card.number = "375001000000005";
            card.expDate = "0421";
            card.type = methodOfPaymentTypeEnum.AX;
            auth.card = card;

            authorizationResponse authorizeResponse = cnp.Authorize(auth);
            Assert.AreEqual("000", authorizeResponse.response);
            Assert.AreEqual("Approved", authorizeResponse.message);
            Assert.AreEqual("44444", authorizeResponse.authCode.Trim());
            Assert.AreEqual("13", authorizeResponse.fraudResult.avsResult);

            capture capture = new capture();
            capture.id = authorizeResponse.id;
            capture.cnpTxnId = authorizeResponse.cnpTxnId;
            capture.amount = 20020;
            captureResponse captureResponse = cnp.Capture(capture);
            Assert.AreEqual("000", captureResponse.response);
            Assert.AreEqual("Approved", captureResponse.message);

            authReversal reversal = new authReversal();
            reversal.id = capture.id;
            reversal.cnpTxnId = authorizeResponse.cnpTxnId;
            reversal.amount = 20020;
            authReversalResponse reversalResponse = cnp.AuthReversal(reversal);
            Assert.AreEqual("000", reversalResponse.response);
            Assert.AreEqual("Approved", reversalResponse.message);
        }

        [Test]
        public void Test36()
        {
            authorization auth = new authorization();
            auth.id = "1";
            auth.orderId = "36";
            auth.amount = 20500;
            auth.orderSource = orderSourceType.ecommerce;
            cardType card = new cardType();
            card.number = "375000026600004";
            card.expDate = "0512";
            card.type = methodOfPaymentTypeEnum.AX;
            auth.card = card;

            authorizationResponse authorizeResponse = cnp.Authorize(auth);
            Assert.AreEqual("000", authorizeResponse.response);
            Assert.AreEqual("Approved", authorizeResponse.message);

            authReversal reversal = new authReversal();
            reversal.id = authorizeResponse.id;
            reversal.cnpTxnId = 360000000000000000;
            reversal.amount = 10000;
            authReversalResponse reversalResponse = cnp.AuthReversal(reversal);
            Assert.AreEqual("000", reversalResponse.response);
            Assert.AreEqual("Approved", reversalResponse.message);
        }            
    }
}
