using Cnp.Sdk.Utlities;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace Cnp.Sdk.Core
{
    public class CommunicationsHttpClientHandler : HttpClientHandler
    {
        public CommunicationsHttpClientHandler(Dictionary<string, string> config, X509Certificate2 clientCertificate = null)
        {
            if (clientCertificate != null)
            {
                ClientCertificateOptions = ClientCertificateOption.Manual;
                ClientCertificates.Add(clientCertificate);
            }

            SslProtocols = System.Security.Authentication.SslProtocols.Tls12;

            // Set the maximum connections for the client, if specified
            if (Config.IsValidConfigValueSet("maxConnections", config))
            {
                int.TryParse(config["maxConnections"], out var maxConnections);
                if (maxConnections > 0)
                {
                    MaxConnectionsPerServer = maxConnections;
                }
            }

            // Configure the client to use the proxy, if specified
            if (Config.IsValidConfigValueSet("proxyHost", config) &&
                Config.IsValidConfigValueSet("proxyPort", config))
            {
                Proxy = new WebProxy(config["proxyHost"], int.Parse(config["" +
                    "proxyPort" +
                    ""]))
                {
                    BypassProxyOnLocal = true
                };
                UseProxy = true;
            }
        }
    }
}
