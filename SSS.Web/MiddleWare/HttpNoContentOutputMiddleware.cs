using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace SSS.Web.MiddleWare
{
    public class HttpNoContentOutputMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpNoContentOutputMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            await _next(httpContext);

            //check if a get request had no content
            if (string.Equals(httpContext.Request.Method, "GET", System.StringComparison.OrdinalIgnoreCase) && httpContext.Response.StatusCode == 204)
            {
                //there is no content on a get request so return a 404
                httpContext.Response.StatusCode = 404;
            }

            //if(httpContext.Response.StatusCode == 404)
            //{
            //    //TODO: log path
            //}
        }
    }

    /// <summary>
    /// Extension method used to add the error handling middleware to the HTTP request pipeline
    /// </summary>
    public static class HttpNoContentOutputMiddlewareExtensions
    {
        /// <summary>
        /// sets status code to 404 if a get request returns with no content
        /// </summary>
        public static IApplicationBuilder UseHttpNoContentOutputMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HttpNoContentOutputMiddleware>();
        }
    }
}
