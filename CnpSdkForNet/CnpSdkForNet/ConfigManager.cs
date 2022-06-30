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
            Timeout = String.IsNullOrEmpty(Properties.Settings.Default.timeout) == false ?
                Int32.Parse(Properties.Settings.Default.timeout) :
                60000,
            ProxyHost = Properties.Settings.Default.proxyHost,
            MerchantId = Properties.Settings.Default.merchantId,
            Password = Properties.Settings.Default.password,
            ProxyPort = String.IsNullOrEmpty(Properties.Settings.Default.proxyPort) == false ? 
                Int32.Parse(Properties.Settings.Default.proxyPort) :
                0,
            NeuterAccountNums = String.IsNullOrEmpty(Properties.Settings.Default.neuterAccountNums) == false ?
                Boolean.Parse(Properties.Settings.Default.neuterAccountNums) :
                false,
            SftpUrl = Properties.Settings.Default.sftpUrl,
            SftpUsername = Properties.Settings.Default.sftpUsername,
            SftpPassword = Properties.Settings.Default.sftpPassword,
            OnlineBatchUrl = Properties.Settings.Default.onlineBatchUrl,
            OnlineBatchPort = String.IsNullOrEmpty(Properties.Settings.Default.onlineBatchPort) == false ? 
                Int32.Parse(Properties.Settings.Default.onlineBatchPort) :
                0,
            RequestDirectory = Properties.Settings.Default.requestDirectory,
            ResponseDirectory = Properties.Settings.Default.responseDirectory,
            UseEncryption = String.IsNullOrEmpty(Properties.Settings.Default.useEncryption) == false ?
                Boolean.Parse(Properties.Settings.Default.useEncryption) :
                false,
            VantivPublicKeyId = Properties.Settings.Default.vantivPublicKeyId,
            PgpPassphrase = Properties.Settings.Default.pgpPassphrase,
            NeuterUserCredentials = Properties.Settings.Default.neuterUserCredentials != null ?
                Boolean.Parse(Properties.Settings.Default.neuterUserCredentials) :
                false,
            MaxConnections = String.IsNullOrEmpty(Properties.Settings.Default.maxConnections) == false ?
                Int32.Parse(Properties.Settings.Default.maxConnections) :
                0
        }) { }

        public ConfigManager(CnpOnlineConfig config)
        {
            _config = config;
        }
    }
}
