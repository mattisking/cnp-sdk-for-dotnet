using Cnp.Sdk;
using Cnp.Sdk.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Cnp.Sdk.Core
{
    public static class CnpSdk4NetExtensions
    {
        public static IServiceCollection AddCnpSdk4Net(this IServiceCollection services,
            Dictionary<string, string> config, X509Certificate2 cert =  null)
        {
            services.AddHttpClient<ICommunications, Communications>()
                .ConfigurePrimaryHttpMessageHandler(sp =>
                {
                    return new CommunicationsHttpClientHandler(config, cert);
                });

            services.AddSingleton<ICnpOnline, CnpOnline>(sp =>
            {
                var communications = sp.GetRequiredService<ICommunications>();
                var logger = sp.GetService<ILoggerFactory>()
                    .CreateLogger<CnpOnline>();

                return new CnpOnline(communications, config, logger);
            });
            return services;
        }
    }
}
