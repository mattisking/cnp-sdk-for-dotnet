using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Moq;
using Cnp.Sdk.Configuration;

namespace Cnp.Sdk.Test.Unit
{
    [TestFixture]
    class TestRFRRequest
    {
        private RFRRequest rfrRequest;

        private const string timeFormat = "MM-dd-yyyy_HH-mm-ss-ffff_";
        private const string timeRegex = "[0-1][0-9]-[0-3][0-9]-[0-9]{4}_[0-9]{2}-[0-9]{2}-[0-9]{2}-[0-9]{4}_";
        private const string batchNameRegex = timeRegex + "[A-Z]{8}";
        private const string mockFileName = "TheRainbow.xml";
        private const string mockFilePath = "C:\\Somewhere\\\\Over\\\\" + mockFileName;

        private Mock<cnpFile> mockCnpFile;
        private Mock<cnpTime> mockCnpTime;

        [OneTimeSetUp]
        public void setUp()
        {
            mockCnpFile = new Mock<cnpFile>();
            mockCnpTime = new Mock<cnpTime>();

            mockCnpFile.Setup(cnpFile => cnpFile.createRandomFile(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>(), mockCnpTime.Object)).Returns(mockFilePath);
            mockCnpFile.Setup(cnpFile => cnpFile.AppendLineToFile(mockFilePath, It.IsAny<String>())).Returns(mockFilePath);
        }

        [SetUp]
        public void SetUpBeforeTest()
        {
            rfrRequest = new RFRRequest();
        }

        [Test]
        public void TestInitialization()
        {
            CnpOnlineConfig mockConfig = new CnpOnlineConfig()
            {
                Url = "https://www.mockurl.com",
                ReportGroup = "Mock Report Group",
                Username = "mockUser",
                Timeout = 35,
                ProxyHost = "www.mockproxy.com",
                MerchantId = "MOCKID",
                Password = "mockPassword",
                ProxyPort = 3000,
                SftpUrl = "www.mockftp.com",
                SftpUsername = "mockFtpUser",
                SftpPassword = "mockFtpPassword",
                OnlineBatchUrl = "www.mockbatch.com",
                OnlineBatchPort = 4000,
                RequestDirectory = Path.Combine(Path.GetTempPath(), "MockRequests"),
                ResponseDirectory = Path.Combine(Path.GetTempPath(), "MockResponses")
            };

            rfrRequest = new RFRRequest(mockConfig);

            Assert.AreEqual(Path.Combine(Path.GetTempPath(),"MockRequests","Requests") + Path.DirectorySeparatorChar, rfrRequest.getRequestDirectory());
            Assert.AreEqual(Path.Combine(Path.GetTempPath(),"MockResponses","Responses") + Path.DirectorySeparatorChar, rfrRequest.getResponseDirectory());

            Assert.NotNull(rfrRequest.getCnpTime());
            Assert.NotNull(rfrRequest.getCnpFile());
        }

        [Test]
        public void TestSerialize()
        {
            cnpFile mockedCnpFile = mockCnpFile.Object;
            cnpTime mockedCnpTime = mockCnpTime.Object;

            rfrRequest.cnpSessionId = 123456789;
            rfrRequest.setCnpFile(mockedCnpFile);
            rfrRequest.setCnpTime(mockedCnpTime);

            Assert.AreEqual(mockFilePath, rfrRequest.Serialize());

            mockCnpFile.Verify(cnpFile => cnpFile.AppendLineToFile(mockFilePath, "\r\n<RFRRequest xmlns=\"http://www.vantivcnp.com/schema\">"));
            mockCnpFile.Verify(cnpFile => cnpFile.AppendLineToFile(mockFilePath, "\r\n<cnpSessionId>123456789</cnpSessionId>"));
            mockCnpFile.Verify(cnpFile => cnpFile.AppendLineToFile(mockFilePath, "\r\n</RFRRequest>"));
        }

        [Test]
        public void TestAccountUpdateFileRequestData() 
        {
            CnpOnlineConfig mockConfig = new CnpOnlineConfig()
            {
                Url = "https://www.mockurl.com",
                ReportGroup = "Mock Report Group",
                Username = "mockUser",
                Timeout = 35,
                ProxyHost = "www.mockproxy.com",
                MerchantId = "MOCKID",
                Password = "mockPassword",
                ProxyPort = 3000,
                SftpUrl = "www.mockftp.com",
                SftpUsername = "mockFtpUser",
                SftpPassword = "mockFtpPassword",
                OnlineBatchUrl = "www.mockbatch.com",
                OnlineBatchPort = 4000,
                RequestDirectory = "C:\\MockRequests",
                ResponseDirectory = "C:\\MockResponses"
            };

            accountUpdateFileRequestData accountUpdateFileRequest = new accountUpdateFileRequestData(mockConfig);
            accountUpdateFileRequestData accountUpdateFileRequestDefault = new accountUpdateFileRequestData();

            Assert.AreEqual(accountUpdateFileRequestDefault.merchantId, Properties.Settings.Default.merchantId);
            Assert.AreEqual(accountUpdateFileRequest.merchantId, mockConfig.MerchantId);
        }
    }
}
