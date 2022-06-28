using System;
using System.Collections.Generic;
using System.Text;

namespace Cnp.Sdk.Configuration
{
    public class CnpOnlineConfig
    {
        public string Url { get; set; }
        public string ReportGroup { get; set; }
        public string Username { get; set; }
        public int Timeout { get; set; }
        public string ProxyHost { get; set; }
        public string MerchantId { get; set; }
        public string Password { get; set; }
        public int ProxyPort { get; set; }
        public bool NeuterAccountNums { get; set; }
        public string SftpUrl { get; set; }
        public string SftpUsername { get; set; }
        public string SftpPassword { get; set; }
        public string OnlineBatchUrl { get; set; }
        public int OnlineBatchPort { get; set; }
        public string RequestDirectory { get; set; }
        public string ResponseDirectory { get; set; }
        public bool UseEncryption { get; set; }
        public string VantivPublicKeyId { get; set; }
        public string PgpPassphrase { get; set; }
        public bool NeuterUserCredentials { get; set; }
        public int MaxConnections { get; set; }
        public string Apikey { get; set; }
    }
}
