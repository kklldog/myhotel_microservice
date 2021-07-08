using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ocelot.Middleware;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Consul;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using api_gateway.aggregators;
using Microsoft.Extensions.DependencyInjection;
using Winton.Extensions.Configuration.Consul;

namespace api_gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new WebHostBuilder()
              .UseKestrel()
              .UseContentRoot(Directory.GetCurrentDirectory())
              .ConfigureAppConfiguration((hostingContext, config) =>
              {
                  var localconfig = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
                  var consul_server = localconfig["consul_server"];

                  config.AddConsul("gateway/routes.json", op => {
                      op.ConsulConfigurationOptions = cco =>
                      {
                          cco.Address = new Uri(consul_server);
                      };
                      op.ReloadOnChange = true;
                  });

                  config.AddEnvironmentVariables();
              })
              .ConfigureServices(s => {
                  s.AddOcelot()
                  .AddConsul()
                  .AddTransientDefinedAggregator<HotelDetailInfoForMobileAggregator>();
              })
              .ConfigureLogging((hostingContext, logging) =>
              {
                  logging.AddConsole();
               })
              .UseIISIntegration()
              .Configure(app =>
              {
                  app.UseOcelot().Wait();
              })
              .Build()
              .Run();
        }
    
    }
}
