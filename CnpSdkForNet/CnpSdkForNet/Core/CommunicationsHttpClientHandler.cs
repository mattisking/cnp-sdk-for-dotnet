using Cnp.Sdk.Configuration;
using Cnp.Sdk.Utlities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace Cnp.Sdk.Core
{
    public class CommunicationsHttpClientHandler : HttpClientHandler
    {
        public CommunicationsHttpClientHandler(CnpOnlineConfig config, X509Certificate2 clientCertificate = null)
        {
            if (clientCertificate != null)
            {
                ClientCertificateOptions = ClientCertificateOption.Manual;
                ClientCertificates.Add(clientCertificate);
            }

            SslProtocols = System.Security.Authentication.SslProtocols.Tls12;

            // Set the maximum connections for the client, if specified
            if (config.MaxConnections > 0)
            {
                MaxConnectionsPerServer = config.MaxConnections;
            }

            // Configure the client to use the proxy, if specified
            if (String.IsNullOrEmpty(config.ProxyHost) == false &&
                config.ProxyPort != 0)
            {
                Proxy = new WebProxy(config.ProxyHost, config.ProxyPort)
                {
                    BypassProxyOnLocal = true
                };
                UseProxy = true;
            }
        }
    }
}
