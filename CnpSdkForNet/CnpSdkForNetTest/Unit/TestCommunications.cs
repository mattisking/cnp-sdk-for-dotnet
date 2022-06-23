using System.Collections.Generic;
using System.Net.Http;
using Cnp.Sdk.Core;
using Cnp.Sdk.Interfaces;
using NUnit.Framework;

namespace Cnp.Sdk.Test.Unit
{
    [TestFixture]
    internal class TestCommunications
    {
        private Dictionary<string, string> config;
        private ICommunications objectUnderTest;

        [OneTimeSetUp]
        public void SetUpCnp()
        {
            config = new Dictionary<string, string> {["url"] = "https://example.com"};

            var handler = new CommunicationsHttpClientHandler(config);
            objectUnderTest = new Communications(new HttpClient(handler), config);
        }

        //[Test]
        //public void TestSettingProxyPropertiesToNullShouldTurnOffProxy()
        //{
        //    config["proxyHost"] = null;
        //    config["proxyPort"] = null;

        //    Assert.IsFalse(objectUnderTest.IsProxyOn());
        //}

        //[Test]
        //public void TestSettingProxyPropertiesToEmptyShouldTurnOffProxy()
        //{
        //    config["proxyHost"] = "";
        //    config["proxyPort"] = "";

        //    Assert.IsFalse(objectUnderTest.IsProxyOn());
        //}

        //[Test]
        //public void TestSettingLogFileToEmptyShouldTurnOffLogFile()
        //{
        //    config["logFile"] = "";

        //    Assert.IsFalse(objectUnderTest.IsValidConfigValueSet("logFile"));
        //}

        //[Test]
        //public void TestConfigNotPresentInDictionary()
        //{
        //    Assert.IsFalse(objectUnderTest.IsValidConfigValueSet("logFile"));
        //}

        [Test]
        public void TestNeuterUserCredentialsActuallyNeuters()
        {
            string inputXml = "<test><user>abc</user><password>123</password></test>";
            config["neuterUserCredentials"] = "true";

            objectUnderTest.NeuterUserCredentials(ref inputXml);

            Assert.That(inputXml.Contains("<user>xxxxxx</user>"));
            Assert.That(inputXml.Contains("<password>xxxxxxxx</password>"));
        }

        [Test]
        public void TestNeuterUserCredentialsDoesNotNeuter()
        {
            string inputXml = "<test><user>abc</user><password>123</password></test>";
            config["neuterUserCredentials"] = "false";

            objectUnderTest.NeuterUserCredentials(ref inputXml);

            Assert.That(inputXml.Contains("<user>abc</user>"));
            Assert.That(inputXml.Contains("<password>123</password>"));
        }
    }
}
