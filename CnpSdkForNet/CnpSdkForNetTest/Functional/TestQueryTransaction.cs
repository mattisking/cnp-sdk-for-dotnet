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
    internal class TestQueryTransaction
    {
        private CnpOnline _cnp;
        private Mock<ILogger> _mockLogger;
        private ICommunications _communications;

        [OneTimeSetUp]
        public void SetUpCnp()
        {
            _mockLogger = new Mock<ILogger>();

            var configManager = new ConfigManager();
            var config = configManager.getConfig();
            var handler = new CommunicationsHttpClientHandler(config);
            _communications = new Communications(new HttpClient(handler), config);

            _cnp = new CnpOnline(_communications, config, _mockLogger.Object);
        }

        [Test]
        public void SimpleQueryTransaction()
        {
            var query = new queryTransaction
            {
                id = "myId",
                reportGroup = "myReportGroup",
                origId = "Deposit1",
                origActionType = actionTypeEnum.D,
                origCnpTxnId = 54321
            };

            var response = _cnp.QueryTransaction(query);
            var queryResponse = (queryTransactionResponse)response;

            Assert.NotNull(queryResponse);
            Assert.AreEqual("sandbox", response.location);
            Assert.AreEqual("150", queryResponse.response);
            Assert.AreEqual("Original transaction found", queryResponse.message);
            Assert.AreEqual("000", ((captureResponse)queryResponse.results_max10[0]).response);

        }

        [Test]
        public void SimpleQueryTransaction_MultipleResponses()
        {
            var query = new queryTransaction
            {
                id = "myId",
                reportGroup = "myReportGroup",
                origId = "Auth2",
                origActionType = actionTypeEnum.A,
                origCnpTxnId = 54321
            };

            var response = _cnp.QueryTransaction(query);
            var queryResponse = (queryTransactionResponse)response;

            Assert.NotNull(queryResponse);
            Assert.AreEqual("150", queryResponse.response);
            Assert.AreEqual("Original transaction found", queryResponse.message);
            Assert.AreEqual(2, queryResponse.results_max10.Count);

        }

        [Test]
        public void TestQueryTransactionUnavailableResponse()
        {
            var query = new queryTransaction
            {
                id = "myId",
                reportGroup = "myReportGroup",
                origId = "Auth",
                origActionType = actionTypeEnum.A,
                origCnpTxnId = 54321
            };

            var response = _cnp.QueryTransaction(query);
            if(response is queryTransactionResponse)
            {
                Assert.Fail("Unexpected type. Aborting the test");
            }
            else
            {
                var queryResponse = (queryTransactionUnavailableResponse)response;
                Assert.AreEqual("152", queryResponse.response);
                Assert.AreEqual("Original transaction found but response not yet available", queryResponse.message);
            }
            

           
        }

        [Test]
        public void TestQueryTransactionNotFoundResponse()
        {
            var query = new queryTransaction
            {
                id = "myId",
                reportGroup = "myReportGroup",
                origId = "Auth0",
                origActionType = actionTypeEnum.A,
                origCnpTxnId = 54321
            };

            var response = _cnp.QueryTransaction(query);
            var queryResponse = (queryTransactionResponse)response;

            Assert.AreEqual("151", queryResponse.response);
            Assert.AreEqual("Original transaction not found", queryResponse.message);
        }
       
        // Add showStatusOnly of type yesNoType as optional
        [Test]
        public void SimpleQueryTransactionShowStatusOnly()
        {
            var query = new queryTransaction
            {
                id = "myId",
                reportGroup = "myReportGroup",
                origId = "Auth1",
                origActionType = actionTypeEnum.A,
                origCnpTxnId = 54321,
                showStatusOnly = yesNoTypeEnum.Y
            };

            var response = _cnp.QueryTransaction(query);
            var queryResponse = (queryTransactionResponse)response;

            Assert.NotNull(queryResponse);
            Assert.AreEqual("150", queryResponse.response);
            Assert.AreEqual("Original transaction found", queryResponse.message);
            Assert.AreEqual(1, queryResponse.results_max10.Count);

        }

        [Test]
        public void TestQueryTransactionAsync()
        {
            var query = new queryTransaction
            {
                id = "myId",
                reportGroup = "myReportGroup",
                origId = "Deposit1",
                origActionType = actionTypeEnum.D,
                origCnpTxnId = 54321
            };

            CancellationToken cancellationToken = new CancellationToken(false);
            var response = _cnp.QueryTransactionAsync(query, cancellationToken);
            var queryResponse = (queryTransactionResponse)response.Result;

            Assert.NotNull(queryResponse);
            Assert.AreEqual("150", queryResponse.response);
            Assert.AreEqual("Original transaction found", queryResponse.message);
            Assert.AreEqual("000", ((captureResponse)queryResponse.results_max10[0]).response);

        }
    }
}
