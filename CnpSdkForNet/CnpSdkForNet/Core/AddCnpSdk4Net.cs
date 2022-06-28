using Cnp.Sdk;
using Cnp.Sdk.Configuration;
using Cnp.Sdk.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Options;

namespace Cnp.Sdk.Core
{
    public static class CnpSdk4NetExtensions
    {
        public static IServiceCollection AddCnpSdk4Net(this IServiceCollection services,
            IConfiguration configuration, X509Certificate2 cert =  null)
        {
            services.Configure<CnpOnlineConfig>(configuration.GetSection("CnpOnlineConfig"));
            services.TryAddSingleton(sp => sp.GetRequiredService<IOptions<CnpOnlineConfig>>().Value);

            services.AddHttpClient<ICommunications, Communications>()
                .ConfigurePrimaryHttpMessageHandler(sp =>
                {
                    var config = sp.GetService<CnpOnlineConfig>();

                    return new CommunicationsHttpClientHandler(config, cert);
                });

            services.AddSingleton<ICnpOnline, CnpOnline>(sp =>
            {
                var communications = sp.GetService<ICommunications>();
                var logger = sp.GetService<ILoggerFactory>()
                    .CreateLogger<CnpOnline>();
                var config = sp.GetService<CnpOnlineConfig>();

                return new CnpOnline(communications, config, logger);
            });
            return services;
        }
    }
}
