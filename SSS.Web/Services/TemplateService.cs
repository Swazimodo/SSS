using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

using SSS.Utilities.Exceptions;
using SSS.Utilities.Interfaces;


namespace SSS.Web.Services
{
    /// <summary>
    /// Renders html content based on razor templates
    /// </summary>
    public class TemplateService : ITemplateService
    {
        private IRazorViewEngine _viewEngine;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITempDataProvider _tempDataProvider;

        public TemplateService(IRazorViewEngine viewEngine, IServiceProvider serviceProvider, ITempDataProvider tempDataProvider)
        {
            _viewEngine = viewEngine;
            _serviceProvider = serviceProvider;
            _tempDataProvider = tempDataProvider;
        }

        public async Task<string> RenderTemplateAsync<TViewModel>(string viewPath, TViewModel viewModel)
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = _serviceProvider
            };

            return await RenderTemplateAsync(httpContext, viewPath, viewModel);

            //var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            //using (var outputWriter = new StringWriter())
            //{
            //    var viewResult = _viewEngine.FindView(actionContext, viewPath, false);
            //    var viewDictionary = new ViewDataDictionary<TViewModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary());
            //    var tempDataDictionary = new TempDataDictionary(httpContext, _tempDataProvider);

            //    if (!viewResult.Success)
            //    {
            //        throw new TemplateServiceException($"Failed to render template {viewPath} because it was not found.");
            //    }

            //    try
            //    {
            //        var viewContext = new ViewContext(actionContext, viewResult.View, viewDictionary,
            //            tempDataDictionary, outputWriter, new HtmlHelperOptions());
            //        viewContext.ViewData.Model = viewModel;

            //        await viewResult.View.RenderAsync(viewContext);
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new TemplateServiceException("Failed to render template due to a razor engine failure", ex);
            //    }

            //    return outputWriter.ToString();
            //}
        }

        public async Task<string> RenderTemplateAsync<TViewModel>(HttpContext httpContext, string viewPath, TViewModel viewModel)
        {
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            using (var outputWriter = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(actionContext, viewPath, false);
                var viewDictionary = new ViewDataDictionary<TViewModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary());
                var tempDataDictionary = new TempDataDictionary(httpContext, _tempDataProvider);

                if (!viewResult.Success)
                {
                    throw new TemplateServiceException($"Failed to render template {viewPath} because it was not found.");
                }

                try
                {
                    var viewContext = new ViewContext(actionContext, viewResult.View, viewDictionary,
                        tempDataDictionary, outputWriter, new HtmlHelperOptions());
                    viewContext.ViewData.Model = viewModel;

                    await viewResult.View.RenderAsync(viewContext);
                }
                catch (Exception ex)
                {
                    throw new TemplateServiceException("Failed to render template due to a razor engine failure", ex);
                }

                return outputWriter.ToString();
            }
        }
    }
}
