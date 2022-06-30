﻿using System.Collections.Generic;
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
    internal class TestPhysicalCheck
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
        public void PhysicalCheckCredit()
        {
            var physicalCheckCredit = new physicalCheckCredit
            {
                // attributes.
                id = "1",
                reportGroup = "Default Report Group",
                // required child elements.
                amount = 1512l,
                fundingSubmerchantId = "value for fundingSubmerchantId",
                fundsTransferId = "value for fundsTransferId"
            };

            var response = _cnp.PhysicalCheckCredit(physicalCheckCredit);
            Assert.AreEqual("000", response.response);
            Assert.AreEqual("sandbox", response.location);
        }
        
        [Test]
        public void PhysicalCheckCreditWithFundingCustomerId()
        {
            var physicalCheckCredit = new physicalCheckCredit
            {
                // attributes.
                id = "1",
                reportGroup = "Default Report Group",
                // required child elements.
                amount = 1512l,
                fundingSubmerchantId = "value for fundingSubmerchantId",
                fundsTransferId = "value for fundsTransferId",
                fundingCustomerId = "value for fundingCustomerId",
            };

            var response = _cnp.PhysicalCheckCredit(physicalCheckCredit);
            Assert.AreEqual("000", response.response);
        }

        [Test]
        public void TestPhysicalCheckCreditAsync()
        {
            var physicalCheckCredit = new physicalCheckCredit
            {
                // attributes.
                id = "1",
                reportGroup = "Default Report Group",
                // required child elements.
                amount = 1512l,
                fundingSubmerchantId = "value for fundingSubmerchantId",
                fundsTransferId = "value for fundsTransferId"
            };

            CancellationToken cancellationToken = new CancellationToken(false);
            var response = _cnp.PhysicalCheckCreditAsync(physicalCheckCredit, cancellationToken);
            Assert.AreEqual("000", response.Result.response);
            Assert.AreEqual("sandbox", response.Result.location);
        }
        
        [Test]
        public void PhysicalCheckDebit()
        {
            var physicalCheckDebit = new physicalCheckDebit
            {
                // attributes.
                id = "1",
                reportGroup = "Planets",
                // required child elements.
                amount = 1512l,
                fundingSubmerchantId = "value for fundingSubmerchantId",
                fundsTransferId = "value for fundsTransferId"
            };

            var response = _cnp.PhysicalCheckDebit(physicalCheckDebit);
            Assert.AreEqual("000", response.response);
            Assert.AreEqual("sandbox", response.location);
        }

        [Test]
        public void PhysicalCheckDebitWithFundingCustomerId()
        {
            var physicalCheckDebit = new physicalCheckDebit
            {
                // attributes.
                id = "1",
                reportGroup = "Planets",
                // required child elements.
                amount = 1512l,
                fundingSubmerchantId = "value for fundingSubmerchantId",
                fundsTransferId = "value for fundsTransferId",
                fundingCustomerId = "value for fundingCustomerId",
            };

            var response = _cnp.PhysicalCheckDebit(physicalCheckDebit);
            Assert.AreEqual("000", response.response);
        }

        [Test]
        public void TestPhysicalCheckDebitAsync()
        {
            var physicalCheckDebit = new physicalCheckDebit
            {
                // attributes.
                id = "1",
                reportGroup = "Planets",
                // required child elements.
                amount = 1512l,
                fundingSubmerchantId = "value for fundingSubmerchantId",
                fundsTransferId = "value for fundsTransferId"
            };

            CancellationToken cancellationToken = new CancellationToken(false);
            var response = _cnp.PhysicalCheckDebitAsync(physicalCheckDebit, cancellationToken);
            Assert.AreEqual("000", response.Result.response);
            Assert.AreEqual("sandbox", response.Result.location);
        }
    }
}
