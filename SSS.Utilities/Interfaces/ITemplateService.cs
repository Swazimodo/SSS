using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SSS.Utilities.Interfaces
{
    /// <summary>
    /// Renders html content based on razor templates
    /// </summary>
    public interface ITemplateService
    {
        /// <summary>
        /// Renders a template given the provided view model
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="viewPath">Filename of the template to render</param>
        /// <param name="viewModel">View model to use for rendering the template</param>
        /// <returns>Returns the rendered template content</returns>
        Task<string> RenderTemplateAsync<TViewModel>(string viewPath, TViewModel viewModel);

        /// <summary>
        /// Renders a template given the provided view model in the current user's context
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="httpContext">Context to run under</param>
        /// <param name="viewPath">Filename of the template to render</param>
        /// <param name="viewModel">View model to use for rendering the template</param>
        /// <returns></returns>
        Task<string> RenderTemplateAsync<TViewModel>(HttpContext httpContext, string viewPath, TViewModel viewModel);
    }
}
