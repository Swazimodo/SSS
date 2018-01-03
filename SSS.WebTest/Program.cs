using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

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
