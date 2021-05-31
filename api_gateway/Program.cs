using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ocelot.Middleware;
using Ocelot.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
                  config
                      .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                      .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                      .AddJsonFile("routes.json")
                      .AddEnvironmentVariables();
              })
              .ConfigureServices(s => {
                  s.AddOcelot();
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
