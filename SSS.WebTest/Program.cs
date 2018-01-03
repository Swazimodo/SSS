using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SSS.WebTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        /// <summary>
        /// Added a hosting.json config file here to allow the web host to be configurable
        /// </summary>
        public static IWebHost BuildWebHost(string[] args)
        {
            //allow for changing the target environment in configuration
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();
            var hostConfig = builder.GetSection("HostConfiguration").Get<SSS.Web.Configuration.HostConfiguration>();

            IWebHost webHost = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseEnvironment(hostConfig.Environment)
                .CaptureStartupErrors(hostConfig.CaptureStartupErrors)
                .UseWebRoot(hostConfig.WebRoot)
                .Build();

            return webHost;
        }
    }
}
