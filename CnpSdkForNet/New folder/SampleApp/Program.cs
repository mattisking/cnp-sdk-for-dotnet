using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Cnp.Sdk.Core;
using System.IO;
using Cnp.Sdk;
using Cnp.Sdk.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace SampleApp
{
    internal class Program
    {
        async static Task Main(string[] args)
        {
            var services = new ServiceCollection();

            // build config
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            // configure logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
            });

            services.AddCnpSdk4Net(configuration);

            // create service provider
            var serviceProvider = services.BuildServiceProvider();

            // Get the service via DI
            var cnpOnline = serviceProvider.GetRequiredService<ICnpOnline>();

            var creditRequest = new credit()
            {
                id = "1",
                reportGroup = "planets",
                amount = 106,
                orderId = "2111",
                orderSource = orderSourceType.ecommerce,
                card = new cardType
                {
                    type = methodOfPaymentTypeEnum.VI,
                    number = "4100000000000001",
                    expDate = "1210"
                }
            };

            var response = await cnpOnline.CreditAsync(creditRequest, new CancellationToken());

            Console.ReadLine();
        }
    }
}
