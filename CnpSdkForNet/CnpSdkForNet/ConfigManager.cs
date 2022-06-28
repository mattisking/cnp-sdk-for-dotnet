using Cnp.Sdk.Configuration;
using System;
using System.Collections.Generic;

namespace Cnp.Sdk
{
    public class ConfigManager
    {
        // Configuration object containing credentials and settings.
        private CnpOnlineConfig _config;

        public CnpOnlineConfig getConfig()
        {
            return _config;
        }

        public ConfigManager() : this(new CnpOnlineConfig
        {
            Url = Properties.Settings.Default.url,
            ReportGroup = Properties.Settings.Default.reportGroup,
            Username = Properties.Settings.Default.username,
            Printxml = Properties.Settings.Default.printxml != null ?
                Boolean.Parse(Properties.Settings.Default.printxml) :
                false,
            Timeout = Properties.Settings.Default.timeout != null ?
                Int32.Parse(Properties.Settings.Default.timeout) :
                60000,
            ProxyHost = Properties.Settings.Default.proxyHost,
            MerchantId = Properties.Settings.Default.merchantId,
            Password = Properties.Settings.Default.password,
            ProxyPort = Properties.Settings.Default.proxyPort != null ? 
                Int32.Parse(Properties.Settings.Default.proxyPort) :
                0,
            NeuterAccountNums = Properties.Settings.Default.neuterAccountNums != null ?
                Boolean.Parse(Properties.Settings.Default.neuterAccountNums) :
                false,
            SftpUrl = Properties.Settings.Default.sftpUrl,
            SftpUsername = Properties.Settings.Default.sftpUsername,
            SftpPassword = Properties.Settings.Default.sftpPassword,
            OnlineBatchUrl = Properties.Settings.Default.onlineBatchUrl,
            OnlineBatchPort = Properties.Settings.Default.onlineBatchPort != null ? 
                Int32.Parse(Properties.Settings.Default.onlineBatchPort) :
                0,
            RequestDirectory = Properties.Settings.Default.requestDirectory,
            ResponseDirectory = Properties.Settings.Default.responseDirectory,
            UseEncryption = Properties.Settings.Default.useEncryption != null ?
                Boolean.Parse(Properties.Settings.Default.useEncryption) :
                false,
            VantivPublicKeyId = Properties.Settings.Default.vantivPublicKeyId,
            PgpPassphrase = Properties.Settings.Default.pgpPassphrase,
            NeuterUserCredentials = Properties.Settings.Default.neuterUserCredentials != null ?
                Boolean.Parse(Properties.Settings.Default.neuterUserCredentials) :
                false,
            MaxConnections = Properties.Settings.Default.maxConnections != null ?
                Int32.Parse(Properties.Settings.Default.maxConnections) :
                0
        }) { }

        public ConfigManager(CnpOnlineConfig config)
        {
            _config = config;
        }
    }
}
