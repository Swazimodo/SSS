using System.IO;
using Microsoft.AspNetCore.Builder;
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

                    // reload on change is disabled because it would be best practice to 
                    // recycle the application when you are ready to apply the change
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: false);

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

            return webHost;
        }
    }
}
