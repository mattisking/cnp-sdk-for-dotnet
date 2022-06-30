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
    class TestSale
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
        public void TestFraudFilterOverride()
        {
            sale sale = new sale();
            sale.orderId = "12344";
            sale.amount = 2;
            sale.orderSource = orderSourceType.ecommerce;
            sale.reportGroup = "Planets";
            sale.fraudFilterOverride = false;
           
            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<fraudFilterOverride>false</fraudFilterOverride>.*", RegexOptions.Singleline) ))
                .Returns("<cnpOnlineResponse version='8.10' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><saleResponse><cnpTxnId>123</cnpTxnId></saleResponse></cnpOnlineResponse>");
     
            _cnp.Sale(sale);
        }

        [Test]
        public void TestSurchargeAmount()
        {
            sale sale = new sale();
            sale.amount = 2;
            sale.surchargeAmount = 1;
            sale.orderSource = orderSourceType.ecommerce;
            sale.reportGroup = "Planets";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<amount>2</amount>\r\n<surchargeAmount>1</surchargeAmount>\r\n<orderSource>ecommerce</orderSource>.*", RegexOptions.Singleline) ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><saleResponse><cnpTxnId>123</cnpTxnId></saleResponse></cnpOnlineResponse>");

            _cnp.Sale(sale);
        }

        [Test]
        public void TestSurchargeAmount_Optional()
        {
            sale sale = new sale();
            sale.amount = 2;
            sale.orderSource = orderSourceType.ecommerce;
            sale.reportGroup = "Planets";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<amount>2</amount>\r\n<orderSource>ecommerce</orderSource>.*", RegexOptions.Singleline) ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><saleResponse><cnpTxnId>123</cnpTxnId></saleResponse></cnpOnlineResponse>");

            _cnp.Sale(sale);
        }

        [Test]
        public void TestRecurringRequest()
        {
            sale sale = new sale();
            sale.card = new cardType();
            sale.card.type = methodOfPaymentTypeEnum.VI;
            sale.card.number = "4100000000000001";
            sale.card.expDate = "1213";
            sale.orderId = "12344";
            sale.amount = 2;
            sale.orderSource = orderSourceType.ecommerce;
            sale.fraudFilterOverride = true;
            sale.recurringRequest = new recurringRequest();
            sale.recurringRequest.subscription = new subscription();
            sale.recurringRequest.subscription.planCode = "abc123";
            sale.recurringRequest.subscription.numberOfPayments = 12;

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<fraudFilterOverride>true</fraudFilterOverride>\r\n<recurringRequest>\r\n<subscription>\r\n<planCode>abc123</planCode>\r\n<numberOfPayments>12</numberOfPayments>\r\n</subscription>\r\n</recurringRequest>.*", RegexOptions.Singleline) ))
                .Returns("<cnpOnlineResponse version='8.18' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><saleResponse><cnpTxnId>123</cnpTxnId></saleResponse></cnpOnlineResponse>");

            _cnp.Sale(sale);
        }

        [Test]
        public void TestRecurringResponse_Full() {
            String xmlResponse = "<cnpOnlineResponse version='8.18' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><saleResponse><cnpTxnId>123</cnpTxnId><recurringResponse><subscriptionId>12</subscriptionId><responseCode>345</responseCode><responseMessage>Foo</responseMessage><recurringTxnId>678</recurringTxnId></recurringResponse></saleResponse></cnpOnlineResponse>";
            cnpOnlineResponse cnpOnlineResponse = CnpOnline.DeserializeObject(xmlResponse);
            saleResponse saleResponse = (saleResponse)cnpOnlineResponse.saleResponse;

            Assert.AreEqual(123, saleResponse.cnpTxnId);
            Assert.AreEqual(12, saleResponse.recurringResponse.subscriptionId);
            Assert.AreEqual("345", saleResponse.recurringResponse.responseCode);
            Assert.AreEqual("Foo", saleResponse.recurringResponse.responseMessage);
            Assert.AreEqual(678, saleResponse.recurringResponse.recurringTxnId);
        }

        [Test]
        public void TestRecurringResponse_NoRecurringTxnId()
        {
            String xmlResponse = "<cnpOnlineResponse version='8.18' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><saleResponse><cnpTxnId>123</cnpTxnId><recurringResponse><subscriptionId>12</subscriptionId><responseCode>345</responseCode><responseMessage>Foo</responseMessage></recurringResponse></saleResponse></cnpOnlineResponse>";
            cnpOnlineResponse cnpOnlineResponse = CnpOnline.DeserializeObject(xmlResponse);
            saleResponse saleResponse = (saleResponse)cnpOnlineResponse.saleResponse;

            Assert.AreEqual(123, saleResponse.cnpTxnId);
            Assert.AreEqual(12, saleResponse.recurringResponse.subscriptionId);
            Assert.AreEqual("345", saleResponse.recurringResponse.responseCode);
            Assert.AreEqual("Foo", saleResponse.recurringResponse.responseMessage);
            Assert.AreEqual(0,saleResponse.recurringResponse.recurringTxnId);
        }

        [Test]
        public void TestRecurringRequest_Optional()
        {
            sale sale = new sale();
            sale.card = new cardType();
            sale.card.type = methodOfPaymentTypeEnum.VI;
            sale.card.number = "4100000000000001";
            sale.card.expDate = "1213";
            sale.orderId = "12344";
            sale.amount = 2;
            sale.orderSource = orderSourceType.ecommerce;
            sale.fraudFilterOverride = true;

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<fraudFilterOverride>true</fraudFilterOverride>\r\n</sale>.*", RegexOptions.Singleline) ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><saleResponse><cnpTxnId>123</cnpTxnId></saleResponse></cnpOnlineResponse>");

            _cnp.Sale(sale);
        }

        [Test]
        public void Test_CnpInternalRecurringRequest()
        {
            sale sale = new sale();
            sale.card = new cardType();
            sale.card.type = methodOfPaymentTypeEnum.VI;
            sale.card.number = "4100000000000001";
            sale.card.expDate = "1213";
            sale.orderId = "12344";
            sale.amount = 2;
            sale.orderSource = orderSourceType.ecommerce;
            sale.fraudFilterOverride = true;
            sale.cnpInternalRecurringRequest = new cnpInternalRecurringRequest();
            sale.cnpInternalRecurringRequest.subscriptionId = "123";
            sale.cnpInternalRecurringRequest.recurringTxnId = "456";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex("<fraudFilterOverride>true</fraudFilterOverride>\r\n<cnpInternalRecurringRequest>\r\n<subscriptionId>123</subscriptionId>\r\n<recurringTxnId>456</recurringTxnId>\r\n</cnpInternalRecurringRequest>.*", RegexOptions.Singleline) ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><saleResponse><cnpTxnId>123</cnpTxnId></saleResponse></cnpOnlineResponse>");

            _cnp.Sale(sale);
        }

        public void Test_CnpInternalRecurringRequest_Optional()
        {
            sale sale = new sale();
            sale.card = new cardType();
            sale.card.type = methodOfPaymentTypeEnum.VI;
            sale.card.number = "4100000000000001";
            sale.card.expDate = "1213";
            sale.orderId = "12344";
            sale.amount = 2;
            sale.orderSource = orderSourceType.ecommerce;
            sale.fraudFilterOverride = true;

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<fraudFilterOverride>true</fraudFilterOverride>\r\n</sale>.*", RegexOptions.Singleline) ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><saleResponse><cnpTxnId>123</cnpTxnId></saleResponse></cnpOnlineResponse>");

            _cnp.Sale(sale);
        }

        [Test]
        public void TestDebtRepayment_True()
        {
            sale sale = new sale();
            sale.cnpInternalRecurringRequest = new cnpInternalRecurringRequest();
            sale.debtRepayment = true;

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*</cnpInternalRecurringRequest>\r\n<debtRepayment>true</debtRepayment>\r\n</sale>.*", RegexOptions.Singleline) ))
                .Returns("<cnpOnlineResponse version='8.19' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><saleResponse><cnpTxnId>123</cnpTxnId></saleResponse></cnpOnlineResponse>");

            _cnp.Sale(sale);
        }

        [Test]
        public void TestDebtRepayment_False()
        {
            sale sale = new sale();
            sale.cnpInternalRecurringRequest = new cnpInternalRecurringRequest();
            sale.debtRepayment = false;

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*</cnpInternalRecurringRequest>\r\n<debtRepayment>false</debtRepayment>\r\n</sale>.*", RegexOptions.Singleline) ))
                .Returns("<cnpOnlineResponse version='8.19' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><saleResponse><cnpTxnId>123</cnpTxnId></saleResponse></cnpOnlineResponse>");

            _cnp.Sale(sale);
        }

        [Test]
        public void TestDebtRepayment_Optional()
        {
            sale sale = new sale();
            sale.cnpInternalRecurringRequest = new cnpInternalRecurringRequest();

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*</cnpInternalRecurringRequest>\r\n</sale>.*", RegexOptions.Singleline) ))
                .Returns("<cnpOnlineResponse version='8.19' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><saleResponse><cnpTxnId>123</cnpTxnId></saleResponse></cnpOnlineResponse>");

            _cnp.Sale(sale);
        }

        [Test]
        public void TestSecondaryAmount()
        {
            sale sale = new sale();
            sale.amount = 2;
            sale.secondaryAmount = 1;
            sale.orderSource = orderSourceType.ecommerce;
            sale.reportGroup = "Planets";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<amount>2</amount>\r\n<secondaryAmount>1</secondaryAmount>\r\n<orderSource>ecommerce</orderSource>.*", RegexOptions.Singleline) ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><saleResponse><cnpTxnId>123</cnpTxnId></saleResponse></cnpOnlineResponse>");

            _cnp.Sale(sale);
        }

        [Test]
        public void TestApplepayAndWallet()
        {
            sale sale = new sale();
            sale.applepay = new applepayType();
            applepayHeaderType applepayHeaderType = new applepayHeaderType();
            applepayHeaderType.applicationData = "454657413164";
            applepayHeaderType.ephemeralPublicKey = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";
            applepayHeaderType.publicKeyHash = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";
            applepayHeaderType.transactionId = "1234";
            sale.applepay.header = applepayHeaderType;
            sale.applepay.data = "user";
            sale.applepay.signature = "sign";
            sale.applepay.version = "1";
            sale.orderId = "12344";
            sale.amount = 2;
            sale.orderSource = orderSourceType.ecommerce;
            wallet wallet = new Sdk.wallet();
            wallet.walletSourceTypeId = "123";
            sale.wallet = wallet;

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*?<cnpOnlineRequest.*?<sale.*?<applepay>.*?<data>user</data>.*?</applepay>.*?<walletSourceTypeId>123</walletSourceTypeId>.*?</wallet>.*?</sale>.*?", RegexOptions.Singleline) ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><saleResponse><cnpTxnId>123</cnpTxnId></saleResponse></cnpOnlineResponse>");

            _cnp.Sale(sale);
        }

        [Test]
        public void SimpleSaleWithDirectDebit()
        {
            sale sale = new sale();
            sale.id = "1";
            sale.amount = 106;
            sale.cnpTxnId = 123456;
            sale.orderId = "12344";
            sale.orderSource = orderSourceType.ecommerce;
            sepaDirectDebitType directDebitObj = new sepaDirectDebitType();
            directDebitObj.mandateProvider = mandateProviderType.Merchant;
            directDebitObj.sequenceType = sequenceTypeType.FirstRecurring;
            directDebitObj.iban = "123456789123456789";
            sale.sepaDirectDebit = directDebitObj;

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<sepaDirectDebit>\r\n<mandateProvider>Merchant</mandateProvider>\r\n<sequenceType>FirstRecurring</sequenceType>\r\n<iban>123456789123456789</iban>\r\n</sepaDirectDebit>.*", RegexOptions.Singleline) ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><saleResponse><cnpTxnId>123</cnpTxnId></saleResponse></cnpOnlineResponse>");

            _cnp.Sale(sale);
        }

        [Test]
        public void SimpleSaleWithProcessTypeNetIdTranAmt()
        {
            sale sale = new sale();
            sale.id = "1";
            sale.amount = 106;
            sale.cnpTxnId = 123456;
            sale.orderId = "12344";
            sale.orderSource = orderSourceType.ecommerce;
            sale.processingType = processingType.accountFunding;
            sale.originalNetworkTransactionId = "Test";
            sale.originalTransactionAmount = 123;

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<processingType>accountFunding</processingType>\r\n<originalNetworkTransactionId>Test</originalNetworkTransactionId>\r\n<originalTransactionAmount>123</originalTransactionAmount>.*", RegexOptions.Singleline) ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><saleResponse><cnpTxnId>123</cnpTxnId></saleResponse></cnpOnlineResponse>");

            _cnp.Sale(sale);
        }

        [Test]
        public void SimpleSaleWithIdealResponse()
        {
            sale sale = new sale();
            sale.id = "1";
            sale.amount = 106;
            sale.cnpTxnId = 123456;
            sale.orderId = "12344";
            sale.orderSource = orderSourceType.ecommerce;
            idealType idealTypeObj = new idealType();
            idealTypeObj.preferredLanguage = countryTypeEnum.US;
            sale.ideal = idealTypeObj;

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<ideal>\r\n<preferredLanguage>US</preferredLanguage>\r\n</ideal>.*", RegexOptions.Singleline) ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><saleResponse><cnpTxnId>123</cnpTxnId></saleResponse></cnpOnlineResponse>");

            _cnp.Sale(sale);
        }

        [Test]
        public void SimpleSaleWithGiropayResponse()
        {
            sale sale = new sale();
            sale.id = "1";
            sale.amount = 106;
            sale.cnpTxnId = 123456;
            sale.orderId = "12344";
            sale.orderSource = orderSourceType.ecommerce;
            giropayType giropayTypeObj = new giropayType();
            giropayTypeObj.preferredLanguage = countryTypeEnum.US;
            sale.giropay = giropayTypeObj;

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<giropay>\r\n<preferredLanguage>US</preferredLanguage>\r\n</giropay>.*", RegexOptions.Singleline) ))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><saleResponse><cnpTxnId>123</cnpTxnId></saleResponse></cnpOnlineResponse>");

            _cnp.Sale(sale);
        }

        [Test]
        public void SimpleSaleWithSofortResponse()
        {
            sale sale = new sale();
            sale.id = "1";
            sale.amount = 106;
            sale.cnpTxnId = 123456;
            sale.orderId = "12344";
            sale.orderSource = orderSourceType.ecommerce;
            sofortType sofortTypeObj = new sofortType();
            sofortTypeObj.preferredLanguage = countryTypeEnum.US;
            sale.sofort = sofortTypeObj;

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<sofort>\r\n<preferredLanguage>US</preferredLanguage>\r\n</sofort>.*", RegexOptions.Singleline)))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><saleResponse><cnpTxnId>123</cnpTxnId></saleResponse></cnpOnlineResponse>");

            _cnp.Sale(sale);
        }

        [Test]
        public void TestSaleWithMCC()
        {
            sale sale = new sale();
            sale.amount = 2;
            sale.merchantCategoryCode = "0111";
            sale.orderSource = orderSourceType.ecommerce;
            sale.reportGroup = "Planets";

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<amount>2</amount>\r\n<orderSource>ecommerce</orderSource>\r\n<merchantCategoryCode>0111</merchantCategoryCode>.*", RegexOptions.Singleline)))
                .Returns("<cnpOnlineResponse version='8.14' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><saleResponse><cnpTxnId>123</cnpTxnId></saleResponse></cnpOnlineResponse>");

            _cnp.Sale(sale);
        }
        
        [Test]
        public void TestSaleWithLocation()
        {
            sale sale = new sale();
            sale.orderId = "12344";
            sale.amount = 2;
            sale.orderSource = orderSourceType.ecommerce;
            sale.reportGroup = "Planets";
            sale.fraudFilterOverride = false;
           
            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<fraudFilterOverride>false</fraudFilterOverride>.*", RegexOptions.Singleline)))
                .Returns("<cnpOnlineResponse version='8.10' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><saleResponse><cnpTxnId>123</cnpTxnId><location>sandbox</location></saleResponse></cnpOnlineResponse>");
     
            var response = _cnp.Sale(sale);
            
            Assert.NotNull(response);
            Assert.AreEqual("sandbox", response.location);
        }

        [Test]
        public void TestSimpleSaleWithRetailerAddressAndAdditionalCOFdata() ///new testcase 12.24
        {
            sale sale = new sale();
            sale.orderId = "12344";
            sale.amount = 2;
            sale.orderSource = orderSourceType.ecommerce;
            sale.reportGroup = "Planets";
            sale.id = "thisisid";
            sale.businessIndicator = businessIndicatorEnum.fundTransfer;
            sale.crypto = false;
            sale.orderChannel = orderChannelEnum.PHONE;
            sale.fraudCheckStatus = "Not Approved";

            var retailerAddress = new contact();
            retailerAddress.name = "Mikasa Ackerman";
            retailerAddress.addressLine1 = "1st Main Street";
            retailerAddress.city = "Burlington";
            retailerAddress.state = "MA";
            retailerAddress.country = countryTypeEnum.USA;
            retailerAddress.email = "mikasa@cnp.com";
            retailerAddress.zip = "01867-4456";
            retailerAddress.sellerId = "s1234";
            retailerAddress.url = "www.google.com";
            sale.retailerAddress = retailerAddress;

            var additionalCOFData = new additionalCOFData();
            additionalCOFData.totalPaymentCount = "35";
            additionalCOFData.paymentType = paymentTypeEnum.Fixed_Amount;
            additionalCOFData.uniqueId = "12345wereew233";
            additionalCOFData.frequencyOfMIT = frequencyOfMITEnum.BiWeekly;
            additionalCOFData.validationReference = "re3298rhriw4wrw";
            additionalCOFData.sequenceIndicator = 2;

            sale.additionalCOFData = additionalCOFData;

            _mockCommunications.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<zip>01867-4456</zip>.*<email>mikasa@cnp.com</email>.*<sellerId>s1234</sellerId>.*<url>www.google.com</url>.*<frequencyOfMIT>BiWeekly</frequencyOfMIT>.*<orderChannel>PHONE</orderChannel>\r\n<fraudCheckStatus>Not Approved</fraudCheckStatus>\r\n<crypto>false</crypto>.*", RegexOptions.Singleline)))
                .Returns("<cnpOnlineResponse version='8.10' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><saleResponse><cnpTxnId>123</cnpTxnId><location>sandbox</location></saleResponse></cnpOnlineResponse>");

            var response = _cnp.Sale(sale);

            Assert.NotNull(response);
            Assert.AreEqual("sandbox", response.location);
        }
    }
}
