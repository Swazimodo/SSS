using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SSS.WebTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        /// <summary>
        /// Added HostConfiguration section for settings that will be used to configure the web host
        /// </summary>
        public static IWebHost BuildWebHost(string[] args)
        {
            //allow for changing the target environment in configuration
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();
            var hostConfig = builder.GetSection("HostConfiguration").Get<Web.Configuration.HostConfiguration>();

            IWebHost webHost = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseEnvironment(hostConfig.Environment)
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    if (hostConfig.LocalDebug)
                    {
                        logging.AddConsole();
                        logging.AddDebug();
                    }
                })
                //.UseDefaultServiceProvider((context, options) =>
                //{
                //    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                //})
                //.ConfigureServices(services =>
                //{
                //    services.AddTransient<IConfigureOptions<KestrelServerOptions>, KestrelServerOptionsSetup>();
                //})
                .Build();

            //webHost = WebHost.CreateDefaultBuilder(args)
            //    .UseStartup<Startup>()
            //    .UseEnvironment(hostConfig.Environment)
            //    .CaptureStartupErrors(hostConfig.CaptureStartupErrors)
            //    .UseWebRoot(hostConfig.WebRoot)
            //    .Build();

            return webHost;
        }
    }
}
