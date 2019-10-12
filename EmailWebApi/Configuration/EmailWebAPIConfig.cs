using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace EmailWebApi.Configuration
{
    public static class EmailWebAPIConfig
    {
        public static IClusterClient ClientToSilo { get; set; }

        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Formatters.JsonFormatter.SupportedMediaTypes
                .Add(new MediaTypeHeaderValue("text/html")); //return JSON

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                //routeTemplate: "{controller}/{id}",
                routeTemplate: "",
                defaults: new { controller = "" }  //id = RouteParameter.Optional
            );
        }

        public static async Task<string> ConnectToSiloAsync()
        {
            try
            {
                ClientToSilo = await ConnectClientToSiloAsync();
                return "Connected";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private static async Task<IClusterClient> ConnectClientToSiloAsync()
        {
            IClusterClient client;
            client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "dev";
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            await client.Connect();
            return client;
        }
    }
}