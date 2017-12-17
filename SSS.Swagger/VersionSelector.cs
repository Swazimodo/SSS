using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace SSS.Swagger
{
    public static class VersionResolver
    {
        /// <summary>
        /// Parses out the version from the relative path
        /// </summary>
        /// <param name="apiDesc">API Method Description</param>
        /// <param name="version">API target API Version</param>
        /// <returns>true if the method is in the target api version</returns>
        public static bool Resolve(ApiDescription apiDesc, string version)
        {
            if (apiDesc.RelativePath.IndexOf('/') > 0)
            {
                string versionByPath = apiDesc.RelativePath.Split('/')[1];
                return (versionByPath == version);
            }
            return false;
        }
    }
}
