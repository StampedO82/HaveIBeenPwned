using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using EmailGrain;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;

namespace EmailSilo
{
    class Program
    {
        static int Main() => RunMainAsync().Result;

        private static async Task<int> RunMainAsync()
        {
            try
            {
                var host = await StartSilo();
                Console.WriteLine("\n\n Press Enter to terminate...\n\n");
                Console.ReadLine();

                await host.StopAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
                return 1;
            }
        }

        private static async Task<ISiloHost> StartSilo()
        {
            ////define the cluster configuration
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                .Configure<EndpointOptions>
                (
                    options => options.AdvertisedIPAddress = IPAddress.Loopback

                )
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "dev";
                })
                .ConfigureLogging
                (
                    logging => logging.AddConsole()
                )
                .AddAzureBlobGrainStorage
                (
                    "AzureBlobStorage",
                    options =>
                    {
                        options.ConnectionString = @"UseDevelopmentStorage=true"; 
                    }
                )
                .UseDashboard(options => { }) // http://localhost:8080
                .UseAzureTableReminderService(options => options.ConnectionString = @"UseDevelopmentStorage=true")
                .ConfigureApplicationParts
                (
                    parts => parts.AddApplicationPart(typeof(UserEmailGrain).Assembly).WithReferences()
                )
                .ConfigureDefaults();

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }
}
