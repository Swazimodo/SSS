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
}
