using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using SSS.Web.Configuration;
using SSS.Web.Extensions;
using SSS.Web.MiddleWare;

namespace SSS.WebTest
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //register custom config sections
            //use your inherited class here if applicable
            services.Configure<WebSettingsBase>(options => Configuration.GetSection("WebSettings").Bind(options));
            WebSettingsBase settings = Configuration.GetSection("WebSettings").Get<WebSettingsBase>();
            if (settings == null)
                throw new Utilities.Exceptions.ProgramException("Null settings object in Startup");

            //services.Configure<ApplicationRoles>(options => Configuration.GetSection("Roles").Bind(options));

            services.AddMvc()
                .AddSerializerSettings(settings);

            // enable session and specify timeout and max length settings
            services.AddDistributedMemoryCache();
            services.AddSession(c =>
            {
                c.Cookie.Expiration = settings.GetSessionExpirationTimeSpan();
                c.IdleTimeout = TimeSpan.FromMinutes(settings.IdleTimeout);
            });

            //add CSRF checking
            services.AddAntiforgery(settings);

            ////configure API versions
            //services.AddSwaggerGen(c =>
            //{
            //    Swashbuckle.Swagger.Model.Info[] APIs = {
            //        new Swashbuckle.Swagger.Model.Info()
            //        {
            //            Title = "Data fix handler"
            //            , Version = "v1"
            //            //, Contact = new Swashbuckle.Swagger.Model.Contact() { Name = "Sam Nesbitt", Email = "sam.nesbitt@cgi.com" }
            //        }/*,
            //        new Swashbuckle.Swagger.Model.Info()
            //        {
            //            Title = "Data fix handler",
            //            Version = "v2"
            //        }*/
            //    };

            //    c.MultipleApiVersions(APIs, WebHelper.SwaggerHelper.VersionResolver);
            //});

            ////set the active directory access group
            //services.AddAuthorization(options =>
            //{
            //    ApplicationRoles roles = Configuration.GetSection("Roles").Get<ApplicationRoles>();
            //    foreach (var role in roles.GetApplicationRoles())
            //    {
            //        options.AddPolicy(role.RoleName, policy =>
            //        {
            //            policy.RequireAuthenticatedUser();
            //            policy.RequireRole(role.ADGroups);
            //        });
            //    }
            //});

            services.AddTransient<Utilities.Interfaces.ITemplateService, Web.Services.TemplateService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile(Configuration.GetSection("Logging")["PathFormat"]);

            WebSettingsBase settings = Configuration.GetSection("WebSettings").Get<WebSettingsBase>();
            if (settings == null)
                throw new Utilities.Exceptions.ProgramException("Null WebSettingsBase configuration object in Startup");

            //use dev page if we are returning detailed errors
            if (settings.ErrorHandlerSettings.ShowErrors)
                app.UseDeveloperExceptionPage();

            if (settings.IsDevelopment())
            {
                //app.UseBrowserLink(); // several studies have shown this to cause cancer
                //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                app.UseStaticFiles();
            }
            else
            {
                //use browser caching
                //this level of caching will require you to use cache busting techniques
                app.UseStaticFiles(new StaticFileOptions()
                {
                    OnPrepareResponse = (context) =>
                    {
                        context.Context.Response.Headers["Cache-Control"] =
                            "private, max-age=2592000";
                        //context.Context.Response.Headers["Expires"] =
                        //    DateTime.UtcNow.AddHours(12).ToString("R");
                    }
                });
            }

            app.UseSession(new SessionOptions()
            {
                IdleTimeout = TimeSpan.FromMinutes(settings.IdleTimeout)
            });

            //converts 204 to 404 on get requests
            app.UseHttpNoContentOutputMiddleware();

            //apply CSRF checking globally across the API
            if (settings.CSRFSettings.Enabled)
                app.UseAntiforgeryHandlerMiddleware();

            //custom error handler
            app.UseErrorHandlerMiddleware(new ErrorHandlerOptions()
            {
                LogErrorCallback = Common.LogErrorCallback,
                MaxErrorCountCallback = Common.MaxErrorCountCallback,
                WebSettings = settings.ErrorHandlerSettings
            });

            ////setup swagger
            //app.UseSwagger();
            //app.UseSwaggerUi();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
